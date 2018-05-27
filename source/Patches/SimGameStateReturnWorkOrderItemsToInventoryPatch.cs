using System;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(SimGameState), "ReturnWorkOrderItemsToInventory")]
    public static class SimGameStateReturnWorkOrderItemsToInventoryPatch
    {
        public static bool Prefix(SimGameState __instance, WorkOrderEntry entry)
        {
            try
            {
                return EnginePersistence.OnReturnWorkOrder(__instance, entry);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return true;
        }
    }
}