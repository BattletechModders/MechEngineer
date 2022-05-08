using BattleTech;
using Harmony;

namespace MechEngineer.Features.ComponentExplosions.Patches;

[HarmonyPatch(typeof(MechStructureRules), nameof(MechStructureRules.GetPassthroughLocation))]
internal static class MechStructureRules_GetPassthroughLocation_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(ArmorLocation location, ref ArmorLocation __result)
    {
        if (location == ArmorLocation.Head)
        {
            __result = ArmorLocation.CenterTorso;
            return false;
        }
        return true;
    }
}