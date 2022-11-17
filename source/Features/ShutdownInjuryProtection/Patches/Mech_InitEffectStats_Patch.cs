using System;
using BattleTech;
using Harmony;
using MechEngineer.Helper;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.InitEffectStats))]
public static class Mech_InitEffectStats_Patch
{
    [HarmonyPrefix]
    public static void Prefix(Mech __instance)
    {
        try
        {
            if (ShutdownInjuryProtectionFeature.settings.ShutdownInjuryEnabled)
            {
                __instance.StatCollection.ReceiveShutdownInjury().Create();
            }
            if (ShutdownInjuryProtectionFeature.settings.HeatDamageInjuryEnabled)
            {
                __instance.StatCollection.ReceiveHeatDamageInjury().Create();
            }
            if (ShutdownInjuryProtectionFeature.settings.OverheatedOnActivationEndInjuryEnabled)
            {
                __instance.StatCollection.ReceiveOverheatedOnActivationEndInjury().Create();
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}

internal static class StatCollectionExtension
{
    internal static StatisticAdapter<bool> ReceiveShutdownInjury(this StatCollection statCollection)
    {
        return new("ReceiveShutdownInjury", statCollection, false);
    }

    internal static StatisticAdapter<bool> ReceiveHeatDamageInjury(this StatCollection statCollection)
    {
        return new("ReceiveHeatDamageInjury", statCollection, false);
    }

    internal static StatisticAdapter<bool> ReceiveOverheatedOnActivationEndInjury(this StatCollection statCollection)
    {
        return new("ReceiveOverheatedOnActivationEndInjury", statCollection, false);
    }
}
