using BattleTech;

namespace MechEngineer.Features.AccuracyEffects.Patches;

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

        AccuracyEffectsFeature.SetupAccuracyStatistics(__instance.StatCollection);
    }
}
