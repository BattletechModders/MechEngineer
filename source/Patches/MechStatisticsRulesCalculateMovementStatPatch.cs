using System;
using System.Linq;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateMovementStat")]
    public static class MechStatisticsRulesCalculateMovementStatPatch
    {
        // make the mech movement summary stat be calculated using the engine
        public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                return Engine.CalculateMovementStat(mechDef, ref currentValue, ref maxValue);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
                return true;
            }
        }
    }
}