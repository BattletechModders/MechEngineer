using BattleTech;

namespace MechEngineer.Features.ComponentExplosions.Patches;

[HarmonyPatch(typeof(MechStructureRules), nameof(MechStructureRules.GetPassthroughLocation))]
internal static class MechStructureRules_GetPassthroughLocation_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, ArmorLocation location, ref ArmorLocation __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (location == ArmorLocation.Head)
        {
            __result = ArmorLocation.CenterTorso;
            __runOriginal = false;
        }
    }
}