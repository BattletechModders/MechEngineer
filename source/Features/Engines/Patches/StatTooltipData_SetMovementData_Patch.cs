using System;
using BattleTech;
using Harmony;
using Localize;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.OverrideTonnage;
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
                void ReplaceDistance(string text, float meter, string newText = null)
                {
                    var meters = PrecisionUtils.RoundDownToInt(meter);
                    var hexWidth = MechStatisticsRules.Combat.MoveConstants.ExperimentalGridDistance;
                    var hexes = PrecisionUtils.RoundDownToInt(meters / hexWidth);
                    var translatedValue = Strings.T("{0}m / {1} hex", meters, hexes);
                    tooltipData.dataList.Remove(text);
                    tooltipData.dataList.Add(newText ?? text, translatedValue);
                }

                ReplaceDistance(Strings.T("Max Move"), stats.WalkSpeed);
                ReplaceDistance(Strings.T("Max Sprint"), stats.RunSpeed, "<u>" + Strings.T("Max Sprint") + "</u>");
                ReplaceDistance(Strings.T("Max Jump"), stats.JumpDistance);
                tooltipData.dataList.Add(Strings.T("TT Walk MP"), $"{stats.WalkMovementPoint}");
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}