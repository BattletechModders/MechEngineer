using BattleTech;

namespace MechEngineer.Features.CriticalEffects.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.ApplyHeadStructureEffects))]
internal static class Mech_ApplyHeadStructureEffects_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal)
    {
        // handle effects via critical effects and DeathMethod CockpitDestroyed
        __runOriginal = false;
    }
}