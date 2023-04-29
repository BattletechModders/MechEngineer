using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(StatTooltipData), nameof(StatTooltipData.SetMeleeData))]
public static class StatTooltipData_SetMeleeData_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, StatTooltipData __instance, MechDef def)
    {
        if (!__runOriginal)
        {
            return;
        }

        OverrideStatTooltipsFeature.MeleeStat.SetupTooltip(__instance, def);
        __runOriginal = false;
    }
}
