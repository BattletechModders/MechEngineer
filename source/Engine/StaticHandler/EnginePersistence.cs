using System;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    // methods that are used for inv -> mech and mech -> inv
    internal static class EnginePersistence
    {
        // auto strip engine when put back to inventory
        internal static void InventoryWidgetOnAddItem(MechLabInventoryWidget widget, MechLabPanel panel, IMechLabDraggableItem item)
        {
            if (item.ItemType != MechLabDraggableItemType.MechComponentItem)
            {
                return;
            }

            var componentRef = item.ComponentRef;

            var engineRef = componentRef.GetEngineCoreRef();
            if (engineRef == null)
            {
                return;
            }

            //Control.mod.Logger.LogDebug("MechLabInventoryWidget.OnAddItem " + componentRef.Def.Description.Id + " UID=" + componentRef.SimGameUID);

            foreach (var componentDefID in engineRef.GetInternalComponents())
            {
                //Control.mod.Logger.LogDebug("MechLabInventoryWidget.OnAddItem extracting componentDefID=" + componentDefID);
                var @ref = CreateMechComponentRef(componentDefID, panel.sim, panel.dataManager);

                var mechLabItemSlotElement = panel.CreateMechComponentItem(@ref, false, ChassisLocations.None, null);
                widget.OnAddItem(mechLabItemSlotElement, false);
            }
            //engineRef.ClearInternalComponents();

            //SaveEngineState(engineRef, panel);

            widget.RefreshFilterToggles();
        }

        internal static MechComponentRef CreateMechComponentRef(string id, SimGameState sim, DataManager dataManager)
        {
            var def = dataManager.GetObjectOfType<HeatSinkDef>(id, BattleTechResourceType.HeatSinkDef);

            var @ref = new MechComponentRef(def.Description.Id, sim.GenerateSimGameUID(), def.ComponentType, ChassisLocations.None);
            @ref.DataManager = dataManager;
            @ref.SetComponentDef(def);

            return @ref;
        }

        internal static void SaveEngineState(EngineCoreRef engineCoreRef, MechLabPanel panel)
        {
            var oldSimGameUID = engineCoreRef.ComponentRef.SimGameUID;
            var newSimGameUID = engineCoreRef.GetNewSimGameUID();
            //Control.mod.Logger.LogDebug($"oldUID={oldSimGameUID} newUID={newSimGameUID}");
            if (oldSimGameUID != newSimGameUID)
            {
                // Control.mod.Logger.LogDebug("saving new state of engine=" + engineRef.engineDef.Def.Description.Id + " old=" + oldSimGameUID + " new=" + newSimGameUID);

                engineCoreRef.ComponentRef.SetSimGameUID(newSimGameUID);
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
                Traverse traverse;
                if (entry is WorkOrderEntry_InstallComponent install)
                {
                    traverse = Traverse.Create(install);
                }
                else if (entry is WorkOrderEntry_RepairComponent repair)
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

        internal static void AddInternalItemsStat(SimGameState sim, MechComponentRef mechComponentRef, string id, Type type, bool damaged)
        {
            var engineRef = mechComponentRef.GetEngineCoreRef();
            if (engineRef == null)
            {
                return;
            }

            foreach (var componentDefID in engineRef.GetInternalComponents())
            {
                sim.AddItemStat(componentDefID, typeof(HeatSinkDef), damaged);
            }
        }

        internal static void RemoveInternalItemsStat(SimGameState sim, MechComponentRef mechComponentRef, string id, Type type, bool damaged)
        {
            var engineRef = mechComponentRef.GetEngineCoreRef();
            if (engineRef == null)
            {
                return;
            }

            foreach (var componentDefID in engineRef.GetInternalComponents())
            {
                sim.RemoveItemStat(componentDefID, typeof(HeatSinkDef), damaged);
            }
        }

        internal static void FixSimGameUID(SimGameState sim, MechComponentRef componentRef)
        {
            if (sim == null)
            {
                return;
            }

            var engineRef = componentRef.GetEngineCoreRef();
            if (engineRef == null)
            {
                return;
            }

            //Control.mod.Logger.LogDebug($"mechDef={Global.ActiveMechDef?.Name}");
            if (engineRef.UUID != null)
            {
                //Control.mod.Logger.LogDebug($"engineRef.GetNewSimGameUID={engineRef.GetNewSimGameUID()}");
                return;
            }

            engineRef.UUID = sim.GenerateSimGameUID();
            componentRef.SetSimGameUID(engineRef.GetNewSimGameUID());
            //Control.mod.Logger.LogDebug($"componentRef.SimGameUID={componentRef.SimGameUID}");
        }

        internal static void FixSimGameUID(SimGameState sim, MechDef mechDef)
        {
            foreach (var componentRef in mechDef.Inventory)
            {
                FixSimGameUID(sim, componentRef);
            }
        }
    }
}