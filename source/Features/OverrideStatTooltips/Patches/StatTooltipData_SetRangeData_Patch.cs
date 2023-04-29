using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetRangeData))]
public static class StatTooltipData_SetRangeData_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, StatTooltipData __instance, MechDef def)
    {
        if (!__runOriginal)
        {
            return;
        }

        OverrideStatTooltipsFeature.RangeStat.SetupTooltip(__instance, def);
        __runOriginal = false;
    }
}
