using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using Object = System.Object;

namespace MechEngineMod
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

                var result = EngineHeat.DropCheck(__instance, ___mechLab, dragItem, ___localInventory);
                var error = result as ErrorResult;
                var remove = result as RemoveItemResult;
                if (error != null)
                {
                    ___dropErrorMessage = error.errorMessage;
                    ___mechLab.ShowDropErrorMessage(___dropErrorMessage);
                    ___mechLab.OnDrop(eventData);
                    return false;
                }
                else if (remove != null)
                {
                    // remove item and delete it
                    dragItem.thisCanvasGroup.blocksRaycasts = true;
                    dragItem.MountedLocation = ChassisLocations.None;
                    ___mechLab.dataManager.PoolGameObject(MechLabPanel.MECHCOMPONENT_ITEM_PREFAB, dragItem.gameObject);
                    ___mechLab.ClearDragItem(true);
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return true;
        }

        internal class Result
        {
        }

        internal class ErrorResult : Result
        {
            internal ErrorResult(string errorMessage)
            {
                this.errorMessage = errorMessage;
            }

            internal string errorMessage;
        }

        internal class RemoveItemResult : Result
        {
        }
    }
}