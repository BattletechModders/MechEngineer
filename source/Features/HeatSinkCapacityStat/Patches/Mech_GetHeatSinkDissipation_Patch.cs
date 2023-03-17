using BattleTech;

namespace MechEngineer.Features.HeatSinkCapacityStat.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.GetHeatSinkDissipation))]
public static class Mech_GetHeatSinkDissipation_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, ref float __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        __result = 0;
        __runOriginal = false;
    }
}