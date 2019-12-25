using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engines.Helper;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateHeatEfficiencyStat))]
    public static class MechStatisticsRules_CalculateHeatEfficiencyStat_Patch
    {

        public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                var stats = new MechDefHeatEfficiencyStatistics(mechDef);
                MechStatisticsRules_CalculateMovementStat_Patch.SetStatValues(stats.GetStatisticRating(), ref currentValue, ref maxValue);
                return false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return true;
        }
    }
}