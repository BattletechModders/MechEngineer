using System.Linq;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    internal static class EnginePersistence
    {
        // make sure to revert all changes when putting stuff back to the inventory
        internal static void OnAddItem(MechLabInventoryWidget widget, MechLabPanel panel, DataManager dataManager, IMechLabDraggableItem item)
        {
            var componentRef = item.ComponentRef;

            if (item.ItemType != MechLabDraggableItemType.MechComponentItem)
            {
                return;
            }

            if (item.OriginalDropParentType != MechLabDropTargetType.InventoryList)
            {
                return;
            }

            var engineRef = componentRef.GetEngineRef();
            if (engineRef == null)
            {
                return;
            }

            Control.mod.Logger.LogDebug("MechLabInventoryWidget.OnAddItem " + componentRef.Def.Description.Id + " UID=" + componentRef.SimGameUID);

            foreach (var componentTypeID in engineRef.GetInternalComponents())
            {
                widget.OnAddItem(componentTypeID, panel.sim, dataManager);
            }
            engineRef.ClearInternalComponents();

            SaveEngineState(engineRef, panel);

            widget.RefreshFilterToggles();
        }

        internal static void OnAddItem(this MechLabInventoryWidget widget, string id, SimGameState sim, DataManager dataManager)
        {
            var def = dataManager.GetObjectOfType<HeatSinkDef>(id, BattleTechResourceType.HeatSinkDef);

            var @ref = new MechComponentRef(def.Description.Id, sim.GenerateSimGameUID(), def.ComponentType, ChassisLocations.None);
            @ref.DataManager = dataManager;
            @ref.SetComponentDef(def);

            var gear = new ListElementController_InventoryGear();
            gear.InitAndCreate(@ref, dataManager, widget, 1);
            widget.OnAddItem(gear.ItemWidget, false);
        }

        internal static void SaveEngineState(EngineRef engineRef, MechLabPanel panel)
        {
            var oldSimGameUID = engineRef.mechComponentRef.SimGameUID;
            var newSimGameUID = engineRef.GetNewSimGameUID();
            if (oldSimGameUID != newSimGameUID)
            {
                Control.mod.Logger.LogDebug("saving new state of engine=" + engineRef.engineDef.Def.Description.Id + " old=" + oldSimGameUID + " new=" + newSimGameUID);

                engineRef.mechComponentRef.SetSimGameUID(newSimGameUID);
                // the only ones actually relying on SimGameUID are these work orders, so we have to change them
                // hopefully no previous or other working order lists are affected
                ChangeWorkOrderSimGameUID(panel, oldSimGameUID, newSimGameUID);
            }
        }

        private static void ChangeWorkOrderSimGameUID(MechLabPanel panel, string oldSimGameUID, string newSimGameUID)
        {
            if (!panel.IsSimGame || panel.baseWorkOrder == null)
            {
                return;
            }

            foreach (var entry in panel.baseWorkOrder.SubEntries)
            {
                var install = entry as WorkOrderEntry_InstallComponent;
                var repair = entry as WorkOrderEntry_RepairComponent;

                Traverse traverse;
                if (install != null)
                {
                    traverse = Traverse.Create(install);
                }
                else if (repair != null)
                {
                    traverse = Traverse.Create(repair);
                }
                else
                {
                    continue;
                }

                traverse = traverse.Property("ComponentSimGameUID");
                var componentSimGameUID = traverse.GetValue<string>();

                if (componentSimGameUID != oldSimGameUID)
                {
                    continue;
                }

                traverse.SetValue(newSimGameUID);
            }
        }

        internal static bool OnReturnWorkOrder(SimGameState sim, WorkOrderEntry entry)
        {
            var install = entry as WorkOrderEntry_RepairComponent;
            var repair = entry as WorkOrderEntry_InstallComponent;

            string simGameUID;
            if (repair != null)
            {
                simGameUID = repair.ComponentSimGameUID;
            }
            else if (install != null)
            {
                simGameUID = install.ComponentSimGameUID;
            }
            else
            {
                return true;
            }

            var mechComponentRef = sim.WorkOrderComponents.FirstOrDefault(c => c.SimGameUID == simGameUID);
            if (mechComponentRef == null)
            {
                return true;
            }

            var engineRef = mechComponentRef.GetEngineRef();
            if (engineRef == null)
            {
                return true;
            }

            foreach (var componentTypeID in engineRef.GetInternalComponents())
            {
                OnReturn(sim, componentTypeID);
            }

            OnReturn(sim, mechComponentRef.ComponentDefID);
            sim.WorkOrderComponents.Remove(mechComponentRef);

            return false;
        }

        internal static void OnReturn(SimGameState sim, string componentDefID)
        {
            sim.AddItemStat(
                componentDefID,
                typeof(HeatSinkDef),
                false
            );
        }
    }
}