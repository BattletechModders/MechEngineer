using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetHeatData))]
public static class StatTooltipData_SetHeatData_Patch
{
    public static void Postfix(StatTooltipData __instance, MechDef def)
    {
        try
        {
            OverrideStatTooltipsFeature.HeatEfficiencyStat.SetupTooltip(__instance, def);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}