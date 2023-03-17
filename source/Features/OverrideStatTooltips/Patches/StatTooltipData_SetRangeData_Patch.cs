using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetRangeData))]
public static class StatTooltipData_SetRangeData_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(StatTooltipData __instance, MechDef def)
    {
        OverrideStatTooltipsFeature.RangeStat.SetupTooltip(__instance, def);
    }
}
