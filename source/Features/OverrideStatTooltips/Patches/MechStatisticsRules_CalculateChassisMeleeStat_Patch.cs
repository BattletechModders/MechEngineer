using BattleTech;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateChassisMeleeStat))]
public static class MechStatisticsRules_CalculateChassisMeleeStat_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, ref float currentValue, ref float maxValue)
    {
        if (!__runOriginal)
        {
            return;
        }

        MechStatUtils.SetStatValues(0, ref currentValue, ref maxValue);
        __runOriginal = false;
    }
}
