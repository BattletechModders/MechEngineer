using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(SimGameState), "CreateComponentRepairWorkOrder")]
    public static class SimGameState_CreateComponentRepairWorkOrder_Patch
    {
        // this.selectedMech
        public static void Postfix(SimGameState __instance, MechComponentRef mechComponent, bool isOnMech, ref WorkOrderEntry_RepairComponent __result)
        {
            try
            {
                WorkOrderCostsHandler.Shared.ComponentRepairWorkOrder(mechComponent, isOnMech, __result);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}