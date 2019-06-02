using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BattleTech;
using CustomComponents;
using Harmony;

namespace MechEngineer.Features
{
    internal abstract class Feature
    {
        internal abstract bool Enabled { get; }

        // called when the feature is enabled and its patches have been successfully loaded
        internal virtual void SetupFeatureLoaded()
        {
            // noop
        }

        // called setup a feature via resources
        internal virtual void SetupResources(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            // noop
        }
        
        internal bool Loaded { get; set; }

        // TODO remove the feature loaded method and make this virtual again? => only if loading becomes again because of sub features
        // setup a feature using patching
        internal void SetupFeature()
        {
            Loaded = FeatureUtils.Setup(this);

            if (Loaded)
            {
                SetupFeatureLoaded();
            }
        }

        private static class FeatureUtils
        {
            internal static bool Setup(Feature feature)
            {
                var type = feature.GetType();
                var topic = type.Name;
                var enabled = feature.Enabled;
                Control.mod.Logger.LogDebug($"{topic} setting up (Enabled={enabled} Namespace={type.Namespace})");
                if (enabled)
                {
                    var typesInNamespace = FindTypesInNamespace(type).ToList();
                    try
                    {
                        PatchCandidates(type, typesInNamespace);
                    }
                    catch (Exception e)
                    {
                        Control.mod.Logger.LogWarning($"{topic} failed patching", e);
                        return false;
                    }

                    try
                    {
                        RegisterCustomCandidates(type, typesInNamespace);
                    }
                    catch (Exception e)
                    {
                        Control.mod.Logger.LogWarning($"{topic} failed registering customs", e);
                        return false;
                    }

                    Control.mod.Logger.Log($"{topic} enabled");
                }
                else
                {
                    Control.mod.Logger.Log($"{topic} disabled");
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

            private static void RegisterCustomCandidates(Type rootType, List<Type> candidates)
            {
                var customType = typeof(ICustom);
                var types = candidates.Where(type => customType.IsAssignableFrom(type)).ToArray();
                if (types.Length < 1)
                {
                    return;
                }

                foreach (var type in types)
                {
                    Control.mod.Logger.LogDebug($" Custom {type.Name}");
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

                    //Control.mod.Logger.LogDebug($"found {type.Namespace}.{type.Name}");

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
                        Control.mod.Logger.LogDebug($" Patch {type.Name}");
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
                return new Hook(processor.Patch(), processor);
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
}
