using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetDurabilityData))]
public static class StatTooltipData_SetDurabilityData_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(StatTooltipData __instance, MechDef def)
    {
        OverrideStatTooltipsFeature.DurabilityStat.SetupTooltip(__instance, def);
    }
}
