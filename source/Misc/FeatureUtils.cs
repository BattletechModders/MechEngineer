using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;

namespace MechEngineer.Misc
{
    internal static class FeatureUtils
    {
        internal static bool SetupFeature(HarmonyInstance harmony, string topic, bool enabled, params Type[] types)
        {
            if (enabled)
            {
                try
                {
                    PatchTypes(harmony, types);
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

        internal static void PatchTypes(HarmonyInstance harmony, params Type[] types)
        {
            var hooks = new List<Hook>();
            try
            {
                foreach (var type in types)
                {
                    var hook = Patch(harmony, type);
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