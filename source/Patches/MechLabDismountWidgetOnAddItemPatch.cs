using System;
using BattleTech.Data;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechLabDismountWidget), "OnAddItem")]
    public static class MechLabDismountWidgetOnAddItemPatch
    {
        public static void Prefix(MechLabDismountWidget __instance, IMechLabDraggableItem item)
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

                EnginePersistence.DismountWidgetOnAddItem(__instance, panel, item);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
