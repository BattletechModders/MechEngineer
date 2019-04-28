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
                if (Control.settings.ShutdownInjuryEnabled)
                {
                    __instance.StatCollection.ReceiveShutdownInjury(true);
                }
                if (Control.settings.HeatDamageInjuryEnabled)
                {
                    __instance.StatCollection.ReceiveHeatDamageInjury(true);
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
        public static bool ReceiveShutdownInjury(this StatCollection statCollection, bool set = false) 
        {
            const string key = "ReceiveShutdownInjury";
            return statCollection.GetStatistic(key)?.Value<bool>()
                   ?? set && statCollection.AddStatistic(key, false).Value<bool>();
        }

        public static bool ReceiveHeatDamageInjury(this StatCollection statCollection, bool set = false) 
        {
            const string key = "ReceiveHeatDamageInjury";
            return statCollection.GetStatistic(key)?.Value<bool>()
                   ?? set && statCollection.AddStatistic(key, false).Value<bool>();
        }
    }
}