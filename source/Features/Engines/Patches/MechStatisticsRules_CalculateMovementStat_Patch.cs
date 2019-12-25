using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engines.Helper;
using UnityEngine;

namespace MechEngineer.Features.Engines.Patches
{

    [HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateMovementStat))]
    public static class MechStatisticsRules_CalculateMovementStat_Patch
    {
        public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                var stats = new MechDefMovementStatistics(mechDef);
                SetStatValues(stats.GetStatisticRating(), ref currentValue, ref maxValue);
                return false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return true;
        }
        
        internal static void SetStatValues(float fraction, ref float currentValue, ref float maxValue)
        {
            var minValue = 1f;
            maxValue = 10f;
            currentValue = fraction * (maxValue - minValue) + minValue;
            currentValue = Mathf.Max(currentValue, minValue);
            currentValue = Mathf.Min(currentValue, maxValue);
        }
    }
}