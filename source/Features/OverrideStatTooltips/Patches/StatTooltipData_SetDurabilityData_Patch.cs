using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetDurabilityData))]
public static class StatTooltipData_SetDurabilityData_Patch
{
    [HarmonyPostfix]
    public static void Postfix(StatTooltipData __instance, MechDef def)
    {
        try
        {
            OverrideStatTooltipsFeature.DurabilityStat.SetupTooltip(__instance, def);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
