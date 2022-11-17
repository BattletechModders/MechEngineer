using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BattleTech;
using CustomComponents;
using Harmony;
using MechEngineer.Misc;

namespace MechEngineer.Features;

[UsedBy(User.FastJson)]
public interface ISettings
{
    bool Enabled { get; }
    string EnabledDescription { get; }
}

internal interface IFeature
{
    void SetupFeature();
    void SetupFeatureResources(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources);
}

internal abstract class Feature<T> : IFeature where T : ISettings
{
    internal virtual bool Enabled => Settings?.Enabled ?? false;

    // called when the feature is enabled and its patches have been successfully loaded
    protected virtual void SetupFeatureLoaded()
    {
        // noop
    }

    // called setup a feature via resources
    protected virtual void SetupResources(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
    {
        // noop
    }

    internal bool Loaded { get; set; }

    // TODO remove the feature loaded method and make this virtual again? => only if loading becomes again because of sub features
    // setup a feature using patching
    public void SetupFeature()
    {
        Loaded = FeatureUtils.Setup(this);

        if (Loaded)
        {
            SetupFeatureLoaded();
        }
    }

    public void SetupFeatureResources(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
    {
        if (Loaded)
        {
            SetupResources(customResources);
        }
    }

    internal abstract T Settings { get; }

    private static class FeatureUtils
    {
        internal static bool Setup<TS>(Feature<TS> feature) where TS : ISettings
        {
            var type = feature.GetType();
            var topic = type.Name;
            var enabled = feature.Enabled;
            Log.Main.Debug?.Log($"{topic} setting up (Enabled={enabled} Namespace={type.Namespace})");
            if (enabled)
            {
                var typesInNamespace = FindTypesInNamespace(type).ToList();
                try
                {
                    PatchCandidates(type, typesInNamespace);
                }
                catch (Exception e)
                {
                    Log.Main.Warning?.Log($"{topic} failed patching", e);
                    return false;
                }

                try
                {
                    RegisterCustomCandidates(typesInNamespace);
                }
                catch (Exception e)
                {
                    Log.Main.Warning?.Log($"{topic} failed registering customs", e);
                    return false;
                }

                Log.Main.Info?.Log($"{topic} enabled");
            }
            else
            {
                Log.Main.Info?.Log($"{topic} disabled");
            }

            return enabled;
        }

        private static void PatchCandidates(Type rootType, List<Type> candidates)
        {
            var types = candidates.Where(type => type.GetCustomAttributes(typeof(HarmonyPatch), true).Length > 0).ToList();
            if (types.Count < 1)
            {
                return;
            }

            //HarmonyInstance.DEBUG = true;
            var harmony = HarmonyInstance.Create(rootType.Namespace);
            PatchTypes(harmony, types);
        }

        private static void RegisterCustomCandidates(List<Type> candidates)
        {
            var customType = typeof(ICustom);
            var types = candidates.Where(type => customType.IsAssignableFrom(type)).ToArray();
            if (types.Length < 1)
            {
                return;
            }

            foreach (var type in types)
            {
                Log.Main.Debug?.Log($" Custom {type.Name}");
                Registry.RegisterSimpleCustomComponents(type);
            }
        }

        private static IEnumerable<Type> FindTypesInNamespace(Type rootType)
        {
            foreach (var type in rootType.Assembly.GetTypes())
            {
                if (rootType.Namespace == null)
                {
                    throw new InvalidOperationException();
                }

                if (type.Namespace == null)
                {
                    continue;
                }

                if (!type.Namespace.StartsWith(rootType.Namespace))
                {
                    continue;
                }

                // Control.Logger.Trace?.Log($"found {type.Namespace}.{type.Name}");

                yield return type;
            }
        }

        private static void PatchTypes(HarmonyInstance harmony, List<Type> types)
        {
            var hooks = new List<Hook>();
            foreach (var type in types)
            {
                try
                {
                    Log.Main.Debug?.Log($" Patch {type.Name}");
                    var hook = Patch(harmony, type);
                    hooks.Add(hook);
                }
                catch
                {
                    foreach (var hook in hooks)
                    {
                        hook.UnPatch();
                    }

                    throw;
                }
            }
        }

        private static Hook Patch(HarmonyInstance harmony, Type type)
        {
            var parentMethodInfos = type.GetHarmonyMethods();
            if (parentMethodInfos == null || !parentMethodInfos.Any())
            {
                throw new InvalidOperationException();
            }

            var info = HarmonyMethod.Merge(parentMethodInfos);
            var processor = new PatchProcessor(harmony, type, info);
            return new(processor.Patch(), processor);
        }

        private class Hook
        {
            private readonly List<DynamicMethod> patches;
            private readonly PatchProcessor processor;

            public Hook(List<DynamicMethod> patches, PatchProcessor processor)
            {
                this.patches = patches;
                this.processor = processor;
            }

            public void UnPatch()
            {
                foreach (var patch in patches)
                {
                    processor.Unpatch(patch);
                }
            }
        }
    }
}