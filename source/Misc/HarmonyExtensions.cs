using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace MechEngineer
{
    internal static class HarmonyExtensions
    {
        internal static void Patch(this HarmonyInstance @this, Type type)
        {
            var parentMethodInfos = type.GetHarmonyMethods();
            if (parentMethodInfos == null || !parentMethodInfos.Any())
            {
                return;
            }

            var info = HarmonyMethod.Merge(parentMethodInfos);
            var processor = new PatchProcessor(@this, type, info);
            processor.Patch();
        }
    }
}
