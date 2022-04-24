using CustomComponents;
using Harmony;

namespace MechEngineer.Features.Engines.Patches;

[HarmonyPatch(typeof(HardpointExtentions), nameof(HardpointExtentions.GetJJMaxByChassisDef))]
internal static class CC_HardpointExtentions_GetJJMaxByChassisDef_Patch
{
    internal static bool Prefix(ref int __result)
    {
        __result = -1;
        return false;
    }
}