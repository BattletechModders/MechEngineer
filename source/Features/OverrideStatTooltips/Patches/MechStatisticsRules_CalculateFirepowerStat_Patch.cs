using BattleTech;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateFirepowerStat))]
public static class MechStatisticsRules_CalculateFirepowerStat_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechDef mechDef, ref float currentValue, ref float maxValue)
    {
        if (!__runOriginal)
        {
            return;
        }

        var value = OverrideStatTooltipsFeature.FirepowerStat.BarValue(mechDef);
        MechStatUtils.SetStatValues(value, ref currentValue, ref maxValue);
        __runOriginal = false;
    }
}
