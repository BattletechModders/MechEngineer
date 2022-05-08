using BattleTech;
using Harmony;

namespace MechEngineer.Features.CriticalEffects.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.ApplyHeadStructureEffects))]
internal static class Mech_ApplyHeadStructureEffects_Patch
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        // handle effects via critical effects and DeathMethod CockpitDestroyed
        return false;
    }
}