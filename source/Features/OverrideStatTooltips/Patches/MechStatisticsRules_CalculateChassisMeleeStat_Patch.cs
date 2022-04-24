using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateChassisMeleeStat))]
public static class MechStatisticsRules_CalculateChassisMeleeStat_Patch
{
    public static bool Prefix(ChassisDef chassisDef, ref float currentValue, ref float maxValue)
    {
        try
        {
            MechStatUtils.SetStatValues(0, ref currentValue, ref maxValue);
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}