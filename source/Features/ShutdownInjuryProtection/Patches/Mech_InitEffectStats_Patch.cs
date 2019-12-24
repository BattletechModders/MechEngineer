using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches
{
    [HarmonyPatch(typeof(Mech), "InitEffectStats")]
    public static class Mech_InitEffectStats_Patch
    {
        public static void Prefix(Mech __instance)
        {
            try
            {
                if (ShutdownInjuryProtectionFeature.settings.ShutdownInjuryEnabled)
                {
                    __instance.StatCollection.ReceiveShutdownInjury().Create(false);
                }
                if (ShutdownInjuryProtectionFeature.settings.HeatDamageInjuryEnabled)
                {
                    __instance.StatCollection.ReceiveHeatDamageInjury().Create(false);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }

    internal static class StatCollectionExtension
    {
        internal static StatisticHelper<bool> ReceiveShutdownInjury(this StatCollection statCollection)
        {
            return new StatisticHelper<bool>("ReceiveShutdownInjury", statCollection);
        }

        internal static StatisticHelper<bool> ReceiveHeatDamageInjury(this StatCollection statCollection)
        {
            return new StatisticHelper<bool>("ReceiveHeatDamageInjury", statCollection);
        }
    }
}