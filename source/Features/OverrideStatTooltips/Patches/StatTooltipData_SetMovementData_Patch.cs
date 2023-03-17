using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetMovementData))]
public static class StatTooltipData_SetMovementData_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(StatTooltipData __instance, MechDef def)
    {
        OverrideStatTooltipsFeature.MovementStat.SetupTooltip(__instance, def);
    }
}
