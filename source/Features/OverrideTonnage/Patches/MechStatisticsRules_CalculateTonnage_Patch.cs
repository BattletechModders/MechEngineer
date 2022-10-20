using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideTonnage.Patches;

[HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateTonnage))]
public static class MechStatisticsRules_CalculateTonnage_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
    {
        try
        {
            var weights = new Weights(mechDef);
            maxValue = weights.StandardChassisWeightCapacity;
            currentValue = weights.TotalWeight;
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
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
            Control.Logger.Error.Log(e);
        }
    }
}