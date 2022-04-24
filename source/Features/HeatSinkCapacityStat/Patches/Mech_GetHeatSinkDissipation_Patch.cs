using BattleTech;
using Harmony;

namespace MechEngineer.Features.HeatSinkCapacityStat.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.GetHeatSinkDissipation))]
public static class Mech_GetHeatSinkDissipation_Patch
{
    public static bool Prefix(ref float __result)
    {
        __result = 0;
        return false;
    }
}