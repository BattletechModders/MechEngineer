using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetFirepowerData))]
public static class StatTooltipData_SetFirepowerData_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, StatTooltipData __instance, MechDef def)
    {
        if (!__runOriginal)
        {
            return;
        }

        OverrideStatTooltipsFeature.FirepowerStat.SetupTooltip(__instance, def);
        __runOriginal = false;
    }
}
