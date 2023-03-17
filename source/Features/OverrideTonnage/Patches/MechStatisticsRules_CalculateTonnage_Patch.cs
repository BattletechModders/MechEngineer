using System;
using BattleTech;

namespace MechEngineer.Features.OverrideTonnage.Patches;

[HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateTonnage))]
public static class MechStatisticsRules_CalculateTonnage_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, MechDef mechDef, ref float currentValue, ref float maxValue)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            var weights = new Weights(mechDef);
            maxValue = weights.StandardChassisWeightCapacity;
            currentValue = weights.TotalWeight;
            __runOriginal = false;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }

    [HarmonyPostfix]
    public static void Postfix(ref float currentValue)
    {
        try
        {
            currentValue = PrecisionUtils.RoundUp(currentValue, OverrideTonnageFeature.settings.TonnageStandardPrecision);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
