using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetRangeData))]
public static class StatTooltipData_SetRangeData_Patch
{
    public static void Postfix(StatTooltipData __instance, MechDef def)
    {
        try
        {
            OverrideStatTooltipsFeature.RangeStat.SetupTooltip(__instance, def);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}