using System;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechLabInventoryWidget), "CreateInventoryItem")]
    public static class MechLabInventoryWidgetCreateInventoryItemPatch
    {
        public static void Prefix(MechLabInventoryWidget __instance, DataManager ___dataManager, MechComponentRef componentRef)
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

                if (componentRef == null)
                {
                    return;
                }

                EnginePersistence.OnCreateInventoryItem(__instance, panel, ___dataManager, componentRef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}