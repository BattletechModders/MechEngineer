using System;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateTonnage")]
    public static class MechStatisticsRulesCalculateTonnagePatch
    {
        // endo-steel and ferros-fibrous calculations for validation
        public static void Postfix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                currentValue += CalculateTonnageFacade.AdditionalTonnage(mechDef);
                currentValue = Mathf.Min(currentValue, maxValue);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}