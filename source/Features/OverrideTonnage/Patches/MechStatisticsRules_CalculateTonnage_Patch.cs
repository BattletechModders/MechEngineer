using BattleTech;

namespace MechEngineer.Features.OverrideTonnage.Patches;

[HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateTonnage))]
public static class MechStatisticsRules_CalculateTonnage_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechDef mechDef, ref float currentValue, ref float maxValue)
    {
        if (!__runOriginal)
        {
            return;
        }

        var weights = new Weights(mechDef);
        maxValue = weights.StandardChassisWeightCapacity;
        currentValue = weights.TotalWeight;
        __runOriginal = false;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(ref float currentValue)
    {
        currentValue = PrecisionUtils.RoundUp(currentValue, OverrideTonnageFeature.settings.TonnageStandardPrecision);
    }
}
