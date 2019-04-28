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
        internal virtual bool Enabled => true;

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
                if (enabled)
                {
                    try
                    {
                        var types = FindHarmonyPatchTypesInNamespace(type);
                        PatchTypes(types);
                    }
                    catch (Exception e)
                    {
                        Control.mod.Logger.Log($"Feature {topic} failed patching", e);
                        return false;
                    }

                    try
                    {
                        var types = FindCustomTypesInNamespace(type).ToArray();
                        Registry.RegisterSimpleCustomComponents(types);
                    }
                    catch (Exception e)
                    {
                        Control.mod.Logger.Log($"Feature {topic} failed registering customs", e);
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

            private static IEnumerable<Type> FindCustomTypesInNamespace(Type rootType)
            {
                var customType = typeof(ICustom);
                return FindTypesInNamespace(rootType).Where(type => customType.IsAssignableFrom(type));
            }

            private static IEnumerable<Type> FindHarmonyPatchTypesInNamespace(Type rootType)
            {
                return FindTypesInNamespace(rootType).Where(type => type.GetCustomAttributes(typeof(HarmonyPatch), true).Length > 0);
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

                    yield return type;
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
