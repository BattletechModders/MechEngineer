using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetRangeData))]
public static class StatTooltipData_SetRangeData_Patch
{
    [HarmonyPostfix]
    public static void Postfix(StatTooltipData __instance, MechDef def)
    {
        try
        {
            OverrideStatTooltipsFeature.RangeStat.SetupTooltip(__instance, def);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
