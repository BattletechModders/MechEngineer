using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engines.Helper;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateChassisMovementStat))]
    public static class MechStatisticsRules_CalculateChassisMovementStat_Patch
    {
        public static bool Prefix(ChassisDef chassisDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                var movement = new EngineMovement(EngineFeature.settings.EngineRatingForChassisMovementStat, chassisDef.Tonnage);
                var fraction = MechDefMovementStatistics.GetStatisticRating(movement.RunSpeed);
                MechStatisticsRules_CalculateMovementStat_Patch.SetStatValues(fraction, ref currentValue, ref maxValue);
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