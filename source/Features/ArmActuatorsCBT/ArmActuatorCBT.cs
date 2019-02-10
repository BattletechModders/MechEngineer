#define CCDEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using HBS.Extensions;

namespace MechEngineer
{
    [CustomComponent("ArmActuatorCBT")]
    public class ArmActuatorCBT : SimpleCustomComponent, IPreValidateDrop, IReplaceValidateDrop, IOnItemGrabbed,
        IAdjustDescription, IOnInstalled
    {
        public ArmActuatorSlot Slot { get; set; }
        public ArmActuatorSlot MaxSlot { get; set; } = ArmActuatorSlot.Hand;

        public string PreValidateDrop(MechLabItemSlotElement item, LocationHelper location, MechLabHelper mechlab)
        {
            if (mechlab.MechLab.activeMechDef.Chassis.Is<ArmSupportCBT>(out var support))
            {
                var part = support.GetByLocation(location.widget.loadout.Location);
                if (part != null && part.MaxActuator < Slot)
                {
                    return
                        $"Cannot install {item.ComponentRef.Def.Description.Name} mech support only up to {part.MaxActuator} in {location.LocationName}";
                }
            }

            foreach (var slot in location.LocalInventory)
            {
                if (slot.ComponentRef.Is<ArmActuatorCBT>(out var actuator))
                {
                    if (actuator.MaxSlot < Slot)
                        return
                            $"Cannot install {item.ComponentRef.Def.Description.Name} because {slot.ComponentRef.Def.Description.Name} is already installed, remove it first";

                    if (MaxSlot < actuator.Slot)
                        return
                            $"Cannot install {item.ComponentRef.Def.Description.Name} because {actuator.Slot} is already installed, remove it first";
                }
            }

            return string.Empty;
        }

        public string ReplaceValidateDrop(MechLabItemSlotElement drop_item, LocationHelper location, List<IChange> changes)
        {
            var total_slot = Slot;
            var mount_loc = location.widget.loadout.Location;
            var mechlab = location.mechLab;
            var mech = mechlab.activeMechDef;

            CustomComponents.Control.LogDebug(DType.ComponentInstall, $"-- ArmActuator: {Def.Description.Id} {Slot}");

            void add_default(ArmActuatorSlot slot)
            {
                bool add_item(string id)
                {
                    CustomComponents.Control.LogDebug(DType.ComponentInstall, $"---- id:[{id}]");
                    if (string.IsNullOrEmpty(id))
                        return false;

                    var r = DefaultHelper.CreateRef(id, ComponentType.Upgrade, mechlab.dataManager, mechlab.Sim);

                    if (r.Is<ArmActuatorCBT>(out var actuator) && (actuator.Slot & total_slot) == 0)
                    {
                        CustomComponents.Control.LogDebug(DType.ComponentInstall, $"----- actuator fit: install");
                        changes.Add(new AddDefaultChange(mount_loc, DefaultHelper.CreateSlot(r, mechlab)));
                        total_slot = total_slot | actuator.Slot;
                        return true;
                    }

                    CustomComponents.Control.LogDebug(DType.ComponentInstall, $"----- actuator not fit, ski[[");
                    return false;
                }

                CustomComponents.Control.LogDebug(DType.ComponentInstall, $"--- adding {slot} to {total_slot}");

                if (total_slot.HasFlag(slot))
                {
                    CustomComponents.Control.LogDebug(DType.ComponentInstall, $"---- already exist, return");

                    return;
                }

                if (add_item(ArmActuatorCBTHandler.GetDefaultActuator(mech, mount_loc, slot)))
                    return;

                add_item(ArmActuatorCBTHandler.GetDefaultActuator(null, mount_loc, slot));
            }

            foreach (var item in location.LocalInventory.Where(i => i.ComponentRef.Is<ArmActuatorCBT>()))
            {
                var actuator = item.ComponentRef.GetComponent<ArmActuatorCBT>();
                if ((actuator.Slot & Slot) != 0)
                {
                    if (item.ComponentRef.IsModuleFixed(location.mechLab.activeMechDef))
                    {
                        CustomComponents.Control.LogDebug(DType.ComponentInstall, $"--- cannot remove {item.ComponentRef.ComponentDefID}");
                        return
                            $"Cannot install {drop_item.ComponentRef.Def.Description.Name} - {Slot} occuped by {item.ComponentRef.Def.Description.Name}";
                    }

                    CustomComponents.Control.LogDebug(DType.ComponentInstall, $"--- removing {item.ComponentRef.ComponentDefID}");
                    changes.Add(new RemoveChange(location.widget.loadout.Location, item));
                }
                else
                    total_slot = total_slot | actuator.Slot;
            }

            add_default(ArmActuatorSlot.Shoulder);
            add_default(ArmActuatorSlot.Upper);

            if (!total_slot.HasFlag(ArmActuatorSlot.Lower) && total_slot.HasFlag(ArmActuatorSlot.Hand))
            {
                CustomComponents.Control.LogDebug(DType.ComponentInstall, $"--- adding Lower to {total_slot}");
                var change = AddFromInventoryChange.FoundInInventory(mount_loc, new MechLabHelper(mechlab),
                    item => item.Is<ArmActuatorCBT>(out var actuator) && actuator.Slot == ArmActuatorSlot.Lower,
                    item => item.Description.Id == Control.settings.DefaultCBTLower
                );
                if(change != null)
                    changes.Add(change);
                else
                    if (Control.settings.InterruptHandDropIfNoLower)
                        return $"Cannot found LowerArm";
            }
            else
            {
                CustomComponents.Control.LogDebug(DType.ComponentInstall, $"--- Lower exist in {total_slot}, done");
            }

            return string.Empty;
        }

        public string AdjustDescription(string Description)
        {
            Description += "\n<color=#28b463><b>[" + Slot + "]</b></color>";
            if (MaxSlot < ArmActuatorSlot.Hand)
            {
                Description += "\n<color=#c0392b><b>Forbid Actuators up to:" + MaxSlot + "</b></color>";
            }

            return Description;
        }

        public void OnItemGrabbed(IMechLabDraggableItem item, MechLabPanel mechLab, MechLabLocationWidget widget)
        {
            var loc_helper = new LocationHelper(widget);
            var total_slot = ArmActuatorSlot.None;
            var mount_loc = widget.loadout.Location;
            var ml_helper = new MechLabHelper(mechLab);

            CustomComponents.Control.LogDebug(DType.ComponentInstall, $"- ArmActuator: {Def.Description.Id} {Slot}");

            void add_default(ArmActuatorSlot slot)
            {
                bool add_item(string id)
                {
                    if (string.IsNullOrEmpty(id))
                        return false;

                    var r = DefaultHelper.CreateRef(id, ComponentType.Upgrade, mechLab.dataManager, mechLab.Sim);

                    if (r.Is<ArmActuatorCBT>(out var actuator) && (actuator.Slot & total_slot) == 0)
                    {
                        DefaultHelper.AddMechLab(id, ComponentType.Upgrade, ml_helper, mount_loc);
                        total_slot = total_slot | actuator.Slot;
                        return true;
                    }

                    return false;
                }
                CustomComponents.Control.LogDebug(DType.ComponentInstall, $"--- adding {slot} to {total_slot}");

                if (total_slot.HasFlag(slot))
                {
                    CustomComponents.Control.LogDebug(DType.ComponentInstall, $"---- already present");
                    return;
                }

                if (add_item(ArmActuatorCBTHandler.GetDefaultActuator(mechLab.activeMechDef, mount_loc, slot)))
                    return;

                add_item(ArmActuatorCBTHandler.GetDefaultActuator(null, mount_loc, slot));
            }


            foreach (var slotitem in loc_helper.LocalInventory.Where(i => i.ComponentRef.Is<ArmActuatorCBT>()))
            {
                var actuator = slotitem.ComponentRef.GetComponent<ArmActuatorCBT>();
                total_slot = total_slot | actuator.Slot;
            }
            CustomComponents.Control.LogDebug(DType.ComponentInstall, $"-- actuators {total_slot}");

            add_default(ArmActuatorSlot.Shoulder);
            add_default(ArmActuatorSlot.Lower);


            if (total_slot.HasFlag(ArmActuatorSlot.Hand) && !total_slot.HasFlag(ArmActuatorSlot.Lower))
            {
                CustomComponents.Control.LogDebug(DType.ComponentInstall, $"-- Removing hand from {total_slot}");

                var hand = loc_helper.LocalInventory.FirstOrDefault(i =>
                    i.ComponentRef.Is<ArmActuatorCBT>(out var actuator) && actuator.Slot.HasFlag(ArmActuatorSlot.Hand));
                if (hand == null || hand.ComponentRef.IsFixed)
                    return;
                var dragitem = mechLab.DragItem;
                widget.OnRemoveItem(hand, false);
                mechLab.ForceItemDrop(hand);
                ml_helper.SetDragItem(dragitem as MechLabItemSlotElement);
            }
            else
            {
                CustomComponents.Control.LogDebug(DType.ComponentInstall, $"-- hand already removed from {total_slot}");
            }
        }

        public void OnInstalled(WorkOrderEntry_InstallComponent order, SimGameState state, MechDef mech)
        {

        }
    }
}