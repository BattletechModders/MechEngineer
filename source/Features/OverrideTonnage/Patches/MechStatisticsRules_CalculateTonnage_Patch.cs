using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideTonnage.Patches
{
    [HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateTonnage))]
    public static class MechStatisticsRules_CalculateTonnage_Patch
    {
        public static void Postfix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                currentValue += WeightsHandler.Shared.TonnageChanges(mechDef);
                currentValue = PrecisionUtils.RoundUp(currentValue, OverrideTonnageFeature.settings.TonnageStandardPrecision);
                maxValue = mechDef.Chassis.Tonnage;
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}