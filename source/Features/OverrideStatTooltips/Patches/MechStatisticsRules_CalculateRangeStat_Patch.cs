using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateRangeStat))]
public static class MechStatisticsRules_CalculateRangeStat_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
    {
        try
        {
            var value = OverrideStatTooltipsFeature.RangeStat.BarValue(mechDef);
            MechStatUtils.SetStatValues(value, ref currentValue, ref maxValue);
            return false;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
        return true;
    }
}
