﻿using BattleTech;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Helper;

namespace MechEngineer.Features.AutoFix.Patches;

[HarmonyPatch(typeof(JumpJetDef), nameof(JumpJetDef.FromJSON))]
public static class JumpJetDef_FromJSON_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    // reduce upgrade components for the center torso that are 3 or larger
    public static void Postfix(JumpJetDef __instance)
    {
        JumpJetDef_FromJSON(__instance);
    }

    private static void JumpJetDef_FromJSON(JumpJetDef def)
    {
        {
            var statisticData = StatCollectionExtension
                .JumpCapacity(null!)
                .CreateStatisticData(
                    StatCollection.StatOperation.Float_Add,
                    def.JumpCapacity
                );

            def.AddPassiveStatisticEffectIfMissing(statisticData);
        }

        if (EngineFeature.settings.JumpJetDefaultJumpHeat.HasValue)
        {
            var statisticData = StatCollectionExtension
                .JumpHeat(null!)
                .CreateStatisticData(
                    StatCollection.StatOperation.Float_Add,
                    EngineFeature.settings.JumpJetDefaultJumpHeat.Value
                );

            def.AddPassiveStatisticEffectIfMissing(statisticData);
        }
    }
}
