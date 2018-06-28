using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using TMPro;

namespace MechEngineer
{
    public static class UniqueController
    {
        public static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {

            var items_by_category = (
                from itemRef in mechDef.Inventory
                let info = itemRef.GetUniqueItem()
                where info != null
                group info by info.ReplaceTag
                into g
                select new {tag = g.Key, count = g.Count()}
            ).ToDictionary(i => i.tag, i => i.count);

            foreach (var category in Control.settings.UniqueCategories)
            {
                int n = 0;
                if (items_by_category.TryGetValue(category.Tag, out n))
                {
                    if (n > 1)
                    {
                        errorMessages[MechValidationType.InvalidInventorySlots]
                            .Add(string.Format(category.ErrorToMany, category.Tag.ToUpper(), category.Tag));
                    }
                }
                else
                {
                    if (category.Required)
                        errorMessages[MechValidationType.InvalidInventorySlots]
                            .Add(string.Format(category.ErrorMissing, category.Tag.ToUpper(), category.Tag));
                }
            }
        }
    }


    [HarmonyPatch(typeof(MechLabLocationWidget), "OnMechLabDrop")]
    public static class MechLabLocationWidget_OnDrop
    {
        public static bool Prefix(MechLabLocationWidget __instance, ref string ___dropErrorMessage,
            List<MechLabItemSlotElement> ___localInventory,
            int ___usedSlots,
            int ___maxSlots,
            TextMeshProUGUI ___locationName,
            MechLabPanel ___mechLab)
        {
            Control.mod.Logger.Log("Drop on " + ___locationName.text);


            if (!___mechLab.Initialized)
            {
                Control.mod.Logger.Log("not Initialized");

                return false;
            }
            if (___mechLab.DragItem == null)
            {
                Control.mod.Logger.Log("Dragged item: NULL, exit");
                return false;
            }

            var drag_item = ___mechLab.DragItem;


            if (drag_item.ComponentRef == null)
            {
                Control.mod.Logger.Log("Dropped item: NULL, exit");

                return false;
            }

            Control.mod.Logger.Log("Dropped item: " + drag_item.ComponentRef.ComponentDefID);

            UniqueItem new_item_info;
            if (!drag_item.ComponentRef.Def.IsUnique(out new_item_info))
            {
                Control.mod.Logger.Log("Item not Unique, exit");

                return true;
            }

            bool flag = __instance.ValidateAdd(drag_item.ComponentRef);

            Control.mod.Logger.Log(string.Format("Validation: {0} - {1}", flag, ___dropErrorMessage));

            if (!flag && !___dropErrorMessage.EndsWith("Not enough free slots."))
                return true;

            var n = ___localInventory.FindUniqueItem(new_item_info);

            Control.mod.Logger.Log("index = " + n.ToString());

            //if no - continue normal flow(add new or show "not enough slots" message
            if (n < 0)
                return true;

            if (___usedSlots - ___localInventory[n].ComponentRef.Def.InventorySize + drag_item.ComponentRef.Def.InventorySize >
                ___maxSlots)
            {
                return true;
            }

            var old_item = ___localInventory[n];
            __instance.OnRemoveItem(old_item, true);
            ___mechLab.ForceItemDrop(old_item);
            var clear = __instance.OnAddItem(drag_item, true);
            if (__instance.Sim != null)
            {
                WorkOrderEntry_InstallComponent subEntry = __instance.Sim.CreateComponentInstallWorkOrder(
                    ___mechLab.baseWorkOrder.MechID,
                    drag_item.ComponentRef, __instance.loadout.Location, drag_item.MountedLocation);
                ___mechLab.baseWorkOrder.AddSubEntry(subEntry);
            }

            drag_item.MountedLocation = __instance.loadout.Location;
            ___mechLab.ClearDragItem(clear);
            __instance.RefreshHardpointData();
            ___mechLab.ValidateLoadout(false);
            return false;
        }

    }
}