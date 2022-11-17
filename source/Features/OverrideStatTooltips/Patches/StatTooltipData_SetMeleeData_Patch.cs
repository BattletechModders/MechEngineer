using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetMeleeData))]
public static class StatTooltipData_SetMeleeData_Patch
{
    [HarmonyPostfix]
    public static void Postfix(StatTooltipData __instance, MechDef def)
    {
        try
        {
            OverrideStatTooltipsFeature.MeleeStat.SetupTooltip(__instance, def);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
