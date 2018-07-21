using System;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateTonnage")]
    public static class MechStatisticsRules_CalculateTonnage_Patch
    {
        public static void Postfix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                currentValue += WeightsHandler.Shared.TonnageChanges(mechDef);
                currentValue = Mathf.Min(currentValue, maxValue);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}