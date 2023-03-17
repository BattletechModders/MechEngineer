using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetHeatData))]
public static class StatTooltipData_SetHeatData_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(StatTooltipData __instance, MechDef def)
    {
        OverrideStatTooltipsFeature.HeatEfficiencyStat.SetupTooltip(__instance, def);
    }
}
