using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetFirepowerData))]
public static class StatTooltipData_SetFirepowerData_Patch
{
    public static void Postfix(StatTooltipData __instance, MechDef def)
    {
        try
        {
            OverrideStatTooltipsFeature.FirepowerStat.SetupTooltip(__instance, def);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}