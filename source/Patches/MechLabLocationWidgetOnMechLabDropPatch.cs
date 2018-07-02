using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine.EventSystems;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabLocationWidget), "OnMechLabDrop")]
    public static class MechLabLocationWidgetOnMechLabDropPatch
    {
        // only allow one engine part per specific location
        public static bool Prefix(
            MechLabLocationWidget __instance,
            PointerEventData eventData,
            MechLabDropTargetType addToType,
            MechLabPanel ___mechLab,
            List<MechLabItemSlotElement> ___localInventory,
            int ___usedSlots,
            int ___maxSlots,
            ref string ___dropErrorMessage)
        {
            try
            {
                if (___mechLab == null)
                {
                    return false;
                }

                var dragItem = ___mechLab.DragItem as MechLabItemSlotElement;
                if (dragItem == null)
                {
                    return false;
                }

                var result = EngineHeat.ValidateDrop(___mechLab, dragItem, ___localInventory);

                var newComponentDef = dragItem.ComponentRef.Def;
                if (result == null)
                {
                    result = ValidationFacade.ValidateDrop(dragItem, ___localInventory);
                }

                if (result is MechLabDropRemoveDragItemResult)
                {
                    // remove item and delete it
                    dragItem.thisCanvasGroup.blocksRaycasts = true;
                    dragItem.MountedLocation = ChassisLocations.None;
                    ___mechLab.dataManager.PoolGameObject(MechLabPanel.MECHCOMPONENT_ITEM_PREFAB, dragItem.gameObject);
                    ___mechLab.ClearDragItem(true);
                    return false;
                }

                if (result is MechLabDropReplaceItemResult replace)
                {
                    var element = replace.ToReplaceElement;
                    if (___usedSlots - element.ComponentRef.Def.InventorySize + newComponentDef.InventorySize <= ___maxSlots)
                    {
                        __instance.OnRemoveItem(element, true);
                        ___mechLab.ForceItemDrop(element);
                        Traverse.Create(___mechLab).Field("dragItem").SetValue(dragItem);
                    }
                    return true;
                    //result = new MechLabDropErrorResult($"Cannot add {newComponentDef.Description.Name}: Type is already installed");
                }

                if (result is MechLabDropErrorResult error)
                {
                    ___dropErrorMessage = error.errorMessage;
                    ___mechLab.ShowDropErrorMessage(___dropErrorMessage);
                    ___mechLab.OnDrop(eventData);
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return true;
        }
    }

    internal class MechLabDropResult
    {
    }

    internal class MechLabDropReplaceItemResult : MechLabDropResult
    {
        internal MechLabItemSlotElement ToReplaceElement;
    }

    internal class MechLabDropRemoveDragItemResult : MechLabDropResult
    {
    }

    internal class MechLabDropErrorResult : MechLabDropResult
    {
        internal string errorMessage;

        internal MechLabDropErrorResult(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }
    }
}