using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateMovementStat))]
public static class MechStatisticsRules_CalculateMovementStat_Patch
{
    public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
    {
        try
        {
            var value = OverrideStatTooltipsFeature.MovementStat.BarValue(mechDef);
            MechStatUtils.SetStatValues(value, ref currentValue, ref maxValue);
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}