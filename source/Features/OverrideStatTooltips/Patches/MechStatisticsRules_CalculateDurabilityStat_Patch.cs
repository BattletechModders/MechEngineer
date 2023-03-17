using BattleTech;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateDurabilityStat))]
public static class MechStatisticsRules_CalculateDurabilityStat_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechDef mechDef, ref float currentValue, ref float maxValue)
    {
        if (!__runOriginal)
        {
            return;
        }

        var value = OverrideStatTooltipsFeature.DurabilityStat.BarValue(mechDef);
        MechStatUtils.SetStatValues(value, ref currentValue, ref maxValue);
        __runOriginal = false;
    }
}
