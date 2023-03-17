using BattleTech;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Helper;
using UnityEngine;

namespace MechEngineer.Features.AutoFix.Patches;

[HarmonyPatch(typeof(HeatSinkDef), nameof(HeatSinkDef.FromJSON))]
public static class HeatSinkDef_FromJSON_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    // reduce upgrade components for the center torso that are 3 or larger 
    public static void Postfix(HeatSinkDef __instance)
    {
        HeatSinkDef_FromJSON(__instance);
    }

    private static void HeatSinkDef_FromJSON(HeatSinkDef def)
    {
        if (Mathf.Approximately(def.DissipationCapacity, 0))
        {
            return;
        }

        var statisticData = StatCollectionExtension
            .HeatSinkCapacity(null!)
            .CreateStatisticData(
                StatCollection.StatOperation.Int_Add,
                (int)def.DissipationCapacity
            );

        def.AddPassiveStatisticEffectIfMissing(statisticData);
    }
}
