using System;
using BattleTech.Data;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabInventoryWidget), "OnAddItem")]
    public static class MechLabInventoryWidget_OnAddItem_Patch
    {
        public static void Prefix(MechLabInventoryWidget __instance, DataManager ___dataManager, IMechLabDraggableItem item)
        {
            try
            {
                var panel = __instance.ParentDropTarget as MechLabPanel;
                if (panel == null)
                {
                    return;
                }

                if (panel.baseWorkOrder == null)
                {
                    return;
                }

                if (!panel.IsSimGame)
                {
                    return;
                }

                if (item == null)
                {
                    return;
                }

                EnginePersistence.InventoryWidgetOnAddItem(__instance, panel, item);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}