using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "InitEffectStats")]
    public static class Mech_InitEffectStats_Patch2
    {
        public static void Prefix(Mech __instance)
        {
            try
            {
                if (Control.settings.ShutdownInjuryEnabled)
                {
                    __instance.StatCollection.ProtectsAgainstShutdownInjury();
                }
                if (Control.settings.HeatDamageInjuryEnabled)
                {
                    __instance.StatCollection.ProtectsAgainstHeatDamageInjury();
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }

    public static class StatCollectionExtension
    {
        public static Statistic ProtectsAgainstShutdownInjury(this StatCollection statCollection) 
        {
            const string key = "ProtectsAgainstShutdownInjury";
            return statCollection.GetStatistic(key) ?? statCollection.AddStatistic(key, true);
        }

        public static Statistic ProtectsAgainstHeatDamageInjury(this StatCollection statCollection) 
        {
            const string key = "ProtectsAgainstHeatDamageInjury";
            return statCollection.GetStatistic(key) ?? statCollection.AddStatistic(key, true);
        }
    }
}