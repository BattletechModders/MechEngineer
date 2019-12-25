using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;

namespace MechEngineer.Features.AutoFix.Patches
{
    [HarmonyPatch(typeof(JumpJetDef), nameof(JumpJetDef.FromJSON))]
    public static class JumpJetDef_FromJSON_Patch
    {
        // reduce upgrade components for the center torso that are 3 or larger 
        public static void Postfix(JumpJetDef __instance)
        {
            try
            {
                JumpJetDef_FromJSON(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        internal static void JumpJetDef_FromJSON(JumpJetDef def)
        {
            {
                var statisticData = new StatisticEffectData()
                {
                    statName = StatCollectionExtension.JumpCapacity(null).Key,
                    operation = StatCollection.StatOperation.Float_Add,
                    modValue = def.JumpCapacity.ToString(),
                    modType = "System.Single"
                };

                def.AddPassiveStatisticEffectIfMissing(statisticData);
            }

            if (EngineFeature.settings.JumpJetDefaultJumpHeat.HasValue)
            {
                var statisticData = new StatisticEffectData()
                {
                    statName = StatCollectionExtension.JumpHeat(null).Key,
                    operation = StatCollection.StatOperation.Float_Add,
                    modValue = EngineFeature.settings.JumpJetDefaultJumpHeat.ToString(),
                    modType = "System.Single"
                };

                def.AddPassiveStatisticEffectIfMissing(statisticData);
            }
        }
    }
}