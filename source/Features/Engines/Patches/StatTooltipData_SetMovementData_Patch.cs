using BattleTech;
using Harmony;
using Localize;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.OverrideTonnage;
using System;

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
                tooltipData.dataList.Clear();

                static string DistanceToSummary(float meter)
                {
                    var meters = PrecisionUtils.RoundDownToInt(meter);
                    var hexWidth = MechStatisticsRules.Combat.MoveConstants.ExperimentalGridDistance;
                    var hexes = PrecisionUtils.RoundDownToInt(meters / hexWidth);
                    var translatedValue = Strings.T("{0}m / {1} hex", meters, hexes);
                    return translatedValue;
                }

                tooltipData.dataList.Add(Strings.T("Max Move"), DistanceToSummary(stats.WalkSpeed));
                tooltipData.dataList.Add("<u>" + Strings.T("Max Sprint") + "</u>", DistanceToSummary(stats.RunSpeed));
                tooltipData.dataList.Add(Strings.T("Max Jump"), DistanceToSummary(stats.JumpDistance));
                tooltipData.dataList.Add(Strings.T("TT Walk MP"), $"{stats.WalkMovementPoint}");
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}