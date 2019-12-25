using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateHeatEfficiencyStat))]
    public static class MechStatisticsRules_CalculateHeatEfficiencyStat_Patch
    {

        public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                //if (def != null && @this.Is<EngineCoreDef>())
                //{
                //    return EngineHeat.GetEngineHeatDissipation(def);
                //}
                //return false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return true;
        }
    }
}