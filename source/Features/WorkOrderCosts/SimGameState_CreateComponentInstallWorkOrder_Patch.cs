using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(SimGameState), "CreateComponentInstallWorkOrder")]
    public static class SimGameState_CreateComponentInstallWorkOrder_Patch
    {
        public static void Postfix(
            SimGameState __instance,
            MechComponentRef mechComponent,
            ChassisLocations newLocation,
            ChassisLocations previousLocation,
            WorkOrderEntry_InstallComponent __result
            )
        {
            try
            {
                WorkOrderCostsHandler.Shared.ComponentInstallWorkOrder(mechComponent, newLocation, __result);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
