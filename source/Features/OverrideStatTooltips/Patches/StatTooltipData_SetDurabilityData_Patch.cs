using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetDurabilityData))]
public static class StatTooltipData_SetDurabilityData_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, StatTooltipData __instance, MechDef def)
    {
        if (!__runOriginal)
        {
            return;
        }

        OverrideStatTooltipsFeature.DurabilityStat.SetupTooltip(__instance, def);
        __runOriginal = false;
    }
}
