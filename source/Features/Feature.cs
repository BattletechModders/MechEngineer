using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
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
            Log.Main.Trace?.Log($"{topic} setting up (Enabled={enabled} Namespace={type.Namespace})");
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

            var harmony = new Harmony(rootType.Namespace);
            foreach (var type in types)
            {
                try
                {
                    Log.Main.Trace?.Log($" Patch {type.Name}");
                    harmony.CreateClassProcessor(type).Patch();
                }
                catch (Exception e)
                {
                    Log.Main.Error?.Log($"Patch {type.Name} failed", e);
                    harmony.UnpatchSelf();
                    throw;
                }
            }
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
                Log.Main.Trace?.Log($" Custom {type.Name}");
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

                Log.Main.Trace?.Log($"found {type.Namespace}.{type.Name}");

                yield return type;
            }
        }
    }
}