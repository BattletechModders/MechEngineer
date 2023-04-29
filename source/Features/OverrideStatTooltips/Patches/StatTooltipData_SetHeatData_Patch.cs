using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetHeatData))]
public static class StatTooltipData_SetHeatData_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, StatTooltipData __instance, MechDef def)
    {
        if (!__runOriginal)
        {
            return;
        }

        OverrideStatTooltipsFeature.HeatEfficiencyStat.SetupTooltip(__instance, def);
        __runOriginal = false;
    }
}
