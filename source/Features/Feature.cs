using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BattleTech;
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
                if (enabled)
                {
                    try
                    {
                        var types = FindTypesInNamespace(type);
                        PatchTypes(types);
                    }
                    catch (Exception e)
                    {
                        Control.mod.Logger.Log($"Feature {topic} failed patching", e);
                        return false;
                    }

                    Control.mod.Logger.Log($"Feature {topic} enabled");
                }
                else
                {
                    Control.mod.Logger.Log($"Feature {topic} disabled");
                }

                return enabled;
            }

            private static IEnumerable<Type> FindTypesInNamespace(Type rootType, bool includeSubNamespaces = true)
            {
                if (rootType.Namespace == null)
                {
                    throw new InvalidOperationException();
                }
                
                foreach (var type in rootType.Assembly.GetTypes())
                {
                    if (type.Namespace == null)
                    {
                        continue;
                    }

                    if (includeSubNamespaces)
                    {
                        if (!type.Namespace.StartsWith(rootType.Namespace))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (type.Namespace != rootType.Namespace)
                        {
                            continue;
                        }
                    }

                    if (type.GetCustomAttributes(typeof(HarmonyPatch), true).Length > 0) {
                        yield return type;
                    }
                }
            }

            private static void PatchTypes(IEnumerable<Type> types)
            {
                var hooks = new List<Hook>();
                try
                {
                    foreach (var type in types)
                    {
                        var hook = Patch(Control.harmony, type);
                        hooks.Add(hook);
                    }
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
