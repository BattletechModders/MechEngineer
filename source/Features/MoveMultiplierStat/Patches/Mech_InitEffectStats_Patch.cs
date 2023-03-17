using BattleTech;

namespace MechEngineer.Features.MoveMultiplierStat.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.InitEffectStats))]
public static class Mech_InitEffectStats_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, Mech __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        MoveMultiplierStatFeature.Shared.InitEffectStats(__instance);
    }
}
