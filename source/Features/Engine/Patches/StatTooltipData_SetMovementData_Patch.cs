using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engine.StaticHandler;
using UnityEngine;

namespace MechEngineer.Features.Engine.Patches
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

                tooltipData.dataList.Remove("Max Move");
                tooltipData.dataList.Remove("Max Sprint");
                
                var combat = CombatGameConstants.GetInstance(UnityGameInstance.BattleTechGame);
                var hexWidth = combat.MoveConstants.ExperimentalGridDistance;
                __instance.dataList.Add("Max Move", $"{movement.WalkSpeed}m / {Mathf.Round(movement.WalkSpeed / hexWidth)} hex");
                __instance.dataList.Add("Max Sprint", $"{movement.RunSpeed}m / {Mathf.Round(movement.RunSpeed / hexWidth)} hex");
                __instance.dataList.Add("TT Walk MP", $"{movement.MovementPoint}");
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}