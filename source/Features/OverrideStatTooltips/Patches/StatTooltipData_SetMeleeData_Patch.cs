using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetMeleeData))]
public static class StatTooltipData_SetMeleeData_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(StatTooltipData __instance, MechDef def)
    {
        OverrideStatTooltipsFeature.MeleeStat.SetupTooltip(__instance, def);
    }
}
