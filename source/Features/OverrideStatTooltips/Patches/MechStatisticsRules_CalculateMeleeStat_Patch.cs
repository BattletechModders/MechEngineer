using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Patches
{
    [HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateMeleeStat))]
    public static class MechStatisticsRules_CalculateMeleeStat_Patch
    {
        public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                var value = OverrideStatTooltipsFeature.MeleeStat.BarValue(mechDef);
                MechStatUtils.SetStatValues(value, ref currentValue, ref maxValue);
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