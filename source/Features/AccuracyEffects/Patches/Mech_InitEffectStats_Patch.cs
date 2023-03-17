using System;
using BattleTech;

namespace MechEngineer.Features.AccuracyEffects.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.InitEffectStats))]
public static class Mech_InitEffectStats_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, Mech __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            AccuracyEffectsFeature.SetupAccuracyStatistics(__instance.StatCollection);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
