using System;
using BattleTech;
using CustomComponents;
using Harmony;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(StatTooltipData), "SetHeatData")]
    public static class StatTooltipData_SetHeatData_Patch
    {
        public static void Postfix(StatTooltipData __instance, MechDef def)
        {
            try
            {
                var mechDef = def;
                
                //if (mechDef != null && @this.Is<EngineCoreDef>())
                //{
                //    return EngineHeat.GetEngineHeatDissipation(mechDef);
                //}
                //var stats = new MechDefMovementStatistics(mechDef);

                //var tooltipData = __instance;
                //void ReplaceDistance(string text, float meter)
                //{
                //    var meters = Mathf.FloorToInt(meter);
                //    var hexWidth = MechStatisticsRules.Combat.MoveConstants.ExperimentalGridDistance;
                //    var hexes = Mathf.FloorToInt(meters / hexWidth);
                //    var translatedText = Strings.T(text);
                //    var translatedValue = Strings.T("{0}m / {1} hex", meters, hexes);
                //    tooltipData.dataList.Remove(translatedText);
                //    tooltipData.dataList.Add(translatedText, translatedValue);
                //}

                //ReplaceDistance("Max Move", stats.WalkSpeed);
                //ReplaceDistance("Max Sprint", stats.RunSpeed);
                //var jumpCapacity = Jumping.GetJumpCapacity(mechDef);
                //var jumpDistance = EngineMovement.ConvertMPToGameDistance(jumpCapacity);
                //ReplaceDistance("Max Jump", jumpDistance);
                //tooltipData.dataList.Add(Strings.T("TT Walk MP"), $"{stats.WalkMovementPoint}");
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}