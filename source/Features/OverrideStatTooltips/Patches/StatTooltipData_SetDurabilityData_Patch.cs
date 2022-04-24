using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetDurabilityData))]
public static class StatTooltipData_SetDurabilityData_Patch
{
    public static void Postfix(StatTooltipData __instance, MechDef def)
    {
        try
        {
            OverrideStatTooltipsFeature.DurabilityStat.SetupTooltip(__instance, def);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}