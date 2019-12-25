using System;
using BattleTech;
using Harmony;
using Localize;
using MechEngineer.Features.Engines.Helper;
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
                var mechDef = def;
                var stats = new MechDefMovementStatistics(mechDef);

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

                ReplaceDistance("Max Move", stats.WalkSpeed);
                ReplaceDistance("Max Sprint", stats.RunSpeed);
                ReplaceDistance("Max Jump", stats.JumpDistance);
                tooltipData.dataList.Add(Strings.T("TT Walk MP"), $"{stats.WalkMovementPoint}");
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}