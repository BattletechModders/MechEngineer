using System;
using BattleTech;
using Harmony;
using Localize;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.Engines.StaticHandler;
using UnityEngine;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(StatTooltipData), "SetMovementData")]
    public static class StatTooltipData_SetMovementData_Patch
    {

        public static void Postfix(StatTooltipData __instance, MechDef def)
        {
            try
            {
                var movement = def.GetEngineMovement();
                if (movement == null)
                {
                    return;
                }

                var tooltipData = __instance;
                void ReplaceDistance(string text, float meter)
                {
                    var meters = Mathf.FloorToInt(meter);
                    var hexWidth = MechStatisticsRules.Combat.MoveConstants.ExperimentalGridDistance;
                    var hexes = Mathf.FloorToInt(meters / hexWidth);
                    var translatedText = Strings.T(text);
                    var translatedValue = Strings.T("{0}m / {1} hex", meters, hexes);
                    tooltipData.dataList.Remove(translatedText);
                    tooltipData.dataList.Add(translatedText, translatedValue);
                }

                ReplaceDistance("Max Move", movement.WalkMaxSpeed);
                ReplaceDistance("Max Sprint", movement.RunMaxSpeed);
                var jumpCapacity = EngineJumpJet.GetJumpCapacity(def);
                var jumpDistance = EngineMovement.ConvertMPToGameDistance(jumpCapacity);
                ReplaceDistance("Max Jump", jumpDistance);
                tooltipData.dataList.Add(Strings.T("TT Walk MP"), $"{movement.WalkMaxMovementPoint}");
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}