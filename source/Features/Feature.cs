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
        internal virtual Type[] Patches { get; } = { };

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
            Loaded = FeatureUtils.SetupFeature(
                GetType().Name,
                Enabled,
                Patches
            );

            if (Loaded)
            {
                SetupFeatureLoaded();
            }
        }

        private static class FeatureUtils
        {
            internal static bool SetupFeature(string topic, bool enabled, params Type[] types)
            {
                if (enabled)
                {
                    try
                    {
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

            private static void PatchTypes(params Type[] types)
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
