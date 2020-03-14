#define CCDEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using HBS.Extensions;

namespace MechEngineer.Features.ArmActuators
{
    [CustomComponent("ArmActuator")]
    public class ArmActuator : SimpleCustomComponent, IPreValidateDrop, IReplaceValidateDrop, IOnItemGrabbed, IOnInstalled, ISorter
    {
        public ArmActuatorSlot Type { get; set; }

        public ArmActuatorSlot MaxSlot { get; set; } = ArmActuatorSlot.Hand;
        public int Order
        {
            get
            {
                if (Type.HasFlag(ArmActuatorSlot.PartShoulder))
                    return 0;
                if (Type.HasFlag(ArmActuatorSlot.PartUpper))
                    return 1;
                if (Type.HasFlag(ArmActuatorSlot.PartLower))
                    return 2;
                if (Type.HasFlag(ArmActuatorSlot.PartHand))
                    return 3;
                return 0;
            }
        }

        public string PreValidateDrop(MechLabItemSlotElement item, LocationHelper location, MechLabHelper mechlab)
        {
            if (location.widget.loadout.Location != ChassisLocations.LeftArm &&
                location.widget.loadout.Location != ChassisLocations.RightArm)
                return string.Empty;

            if (mechlab.MechLab.activeMechDef.Chassis.Is<ArmActuatorSupport>(out var support))
            {
                var max = support.GetLimit(location.widget.loadout.Location);

                if (max < Type)
                {
                    return
                        $"Cannot install {item.ComponentRef.Def.Description.Name} mech support only up to {max} in {location.LocationName}";
                }
            }

            foreach (var slot in location.LocalInventory)
            {
                if (slot.ComponentRef.Is<ArmActuator>(out var actuator))
                {
                    if (actuator.MaxSlot < Type)
                        return
                            $"Cannot install {item.ComponentRef.Def.Description.Name} because {slot.ComponentRef.Def.Description.Name} is already installed, remove it first";

                    if (MaxSlot < actuator.Type)
                        return
                            $"Cannot install {item.ComponentRef.Def.Description.Name} because {actuator.Type} is already installed, remove it first";
                }
            }

            return string.Empty;
        }

        public string ReplaceValidateDrop(MechLabItemSlotElement drop_item, LocationHelper location, List<IChange> changes)
        {
            if (location.widget.loadout.Location != ChassisLocations.LeftArm &&
                location.widget.loadout.Location != ChassisLocations.RightArm)
                return string.Empty;

            var total_slot = Type;
            var mount_loc = location.widget.loadout.Location;
            var mechlab = location.mechLab;
            var mech = mechlab.activeMechDef;

            CustomComponents.Control.LogDebug(DType.ComponentInstall, $"-- ArmActuator: {Def.Description.Id} {Type}");

            void add_default(ArmActuatorSlot slot)
            {
                bool add_item(string id)
                {
                    CustomComponents.Control.LogDebug(DType.ComponentInstall, $"---- id:[{id}]");
                    if (string.IsNullOrEmpty(id))
                        return false;

                    var r = DefaultHelper.CreateRef(id, ComponentType.Upgrade, mechlab.dataManager, mechlab.Sim);

                    if (r.Is<ArmActuator>(out var actuator) && (actuator.Type & total_slot) == 0)
                    {
                        CustomComponents.Control.LogDebug(DType.ComponentInstall, $"----- actuator fit: install");
                        changes.Add(new AddDefaultChange(mount_loc, DefaultHelper.CreateSlot(r, mechlab)));
                        total_slot = total_slot | actuator.Type;
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

                if (add_item(ArmActuatorCC.GetDefaultActuator(mech, mount_loc, slot)))
                    return;

                add_item(ArmActuatorCC.GetDefaultActuator(null, mount_loc, slot));
            }

            foreach (var item in location.LocalInventory.Where(i => i.ComponentRef.Is<ArmActuator>()))
            {
                var actuator = item.ComponentRef.GetComponent<ArmActuator>();
                if ((actuator.Type & Type) != 0)
                {
                    if (item.ComponentRef.IsModuleFixed(location.mechLab.activeMechDef))
                    {
                        CustomComponents.Control.LogDebug(DType.ComponentInstall,
                            $"--- cannot remove {item.ComponentRef.ComponentDefID}");
                        return
                            $"Cannot install {drop_item.ComponentRef.Def.Description.Name} - {Type} occuped by {item.ComponentRef.Def.Description.Name}";
                    }

                    CustomComponents.Control.LogDebug(DType.ComponentInstall,
                        $"--- removing {item.ComponentRef.ComponentDefID}");
                    changes.Add(new RemoveChange(location.widget.loadout.Location, item));
                }
                else
                    total_slot = total_slot | actuator.Type;
            }

            add_default(ArmActuatorSlot.PartShoulder);
            add_default(ArmActuatorSlot.PartUpper);

            if (ArmActuatorCC.IsIgnoreFullActuators(mech))
            {

                if (!total_slot.HasFlag(ArmActuatorSlot.PartLower) && total_slot.HasFlag(ArmActuatorSlot.PartHand))
                {
                    CustomComponents.Control.LogDebug(DType.ComponentInstall, $"--- adding Lower to {total_slot}");
                    var change = AddFromInventoryChange.FoundInInventory(mount_loc, new MechLabHelper(mechlab),
                        item => item.Is<ArmActuator>(out var actuator) && actuator.Type == ArmActuatorSlot.PartLower,
                        item => item.Description.Id == ArmActuatorFeature.settings.DefaultCBTLower
                    );
                    if (change != null)
                        changes.Add(change);
                    else if (ArmActuatorFeature.settings.InterruptHandDropIfNoLower)
                        return $"Cannot found LowerArm";
                }
                else
                {
                    CustomComponents.Control.LogDebug(DType.ComponentInstall, $"--- Lower exist in {total_slot}, done");
                }
            }
            else
            {
                var limit = mech.Chassis.Is<ArmActuatorSupport>(out var s)
                    ? s.GetLimit(mount_loc)
                    : ArmActuatorSlot.Hand;
                if (limit.HasFlag(ArmActuatorSlot.PartLower))
                    add_default(ArmActuatorSlot.PartLower);
                if (limit.HasFlag(ArmActuatorSlot.PartHand))
                    add_default(ArmActuatorSlot.PartHand);
            }
            return string.Empty;

        }

        public void OnItemGrabbed(IMechLabDraggableItem item, MechLabPanel mechLab, MechLabLocationWidget widget)
        {
            if (widget.loadout.Location != ChassisLocations.LeftArm &&
                widget.loadout.Location != ChassisLocations.RightArm)
                return;

            var loc_helper = new LocationHelper(widget);
            var total_slot = ArmActuatorSlot.None;
            var mount_loc = widget.loadout.Location;
            var ml_helper = new MechLabHelper(mechLab);

            CustomComponents.Control.LogDebug(DType.ComponentInstall, $"- ArmActuator: {Def.Description.Id} {Type}");


            void add_default(ArmActuatorSlot slot)
            {
                bool add_item(string id)
                {
                    if (string.IsNullOrEmpty(id))
                        return false;

                    var r = DefaultHelper.CreateRef(id, ComponentType.Upgrade, mechLab.dataManager, mechLab.Sim);

                    if (r.Is<ArmActuator>(out var actuator) && (actuator.Type & total_slot) == 0)
                    {
                        DefaultHelper.AddMechLab(id, ComponentType.Upgrade, ml_helper, mount_loc);
                        total_slot = total_slot | actuator.Type;
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

                if (add_item(ArmActuatorCC.GetDefaultActuator(mechLab.activeMechDef, mount_loc, slot)))
                    return;

                add_item(ArmActuatorCC.GetDefaultActuator(null, mount_loc, slot));
            }

            for (int i = loc_helper.LocalInventory.Count - 1; i >= 0; i--)
            {
                var slotitem = loc_helper.LocalInventory[i];

                if (!slotitem.ComponentRef.Is<ArmActuator>())
                    continue;

                var actuator = slotitem.ComponentRef.GetComponent<ArmActuator>();
                if (slotitem.ComponentRef.IsDefault() &&
                    !slotitem.ComponentRef.IsModuleFixed(ml_helper.MechLab.activeMechDef))
                {
                    CustomComponents.Control.LogDebug(DType.ComponentInstall,
                        $"-- removing {slotitem.ComponentRef.ComponentDefID} {actuator.Type}");

                    DefaultHelper.RemoveMechLab(slotitem.ComponentRef.ComponentDefID,
                        slotitem.ComponentRef.ComponentDefType, ml_helper, mount_loc);
                }
                else
                {
                    CustomComponents.Control.LogDebug(DType.ComponentInstall,
                        $"-- checking {slotitem.ComponentRef.ComponentDefID} {actuator.Type}");
                    total_slot = total_slot | actuator.Type;
                }
            }

            CustomComponents.Control.LogDebug(DType.ComponentInstall, $"-- actuators {total_slot}");

            add_default(ArmActuatorSlot.PartShoulder);
            add_default(ArmActuatorSlot.PartUpper);

            if (ArmActuatorCC.IsIgnoreFullActuators(mechLab.activeMechDef))
            {
                if (total_slot.HasFlag(ArmActuatorSlot.PartHand) && !total_slot.HasFlag(ArmActuatorSlot.PartLower))
                {
                    CustomComponents.Control.LogDebug(DType.ComponentInstall, $"-- Removing hand from {total_slot}");

                    var hand = loc_helper.LocalInventory.FirstOrDefault(i =>
                        i.ComponentRef.Is<ArmActuator>(out var actuator) &&
                        actuator.Type.HasFlag(ArmActuatorSlot.PartHand));
                    if (hand == null || hand.ComponentRef.IsFixed)
                        return;
                    var dragitem = mechLab.DragItem;
                    widget.OnRemoveItem(hand, false);
                    mechLab.ForceItemDrop(hand);
                    ml_helper.SetDragItem(dragitem as MechLabItemSlotElement);
                }
                else
                {
                    CustomComponents.Control.LogDebug(DType.ComponentInstall,
                        $"-- hand already removed from {total_slot}");
                }
            }
            else
            {
                var limit = mechLab.activeMechDef.Chassis.Is<ArmActuatorSupport>(out var s)
                    ? s.GetLimit(mount_loc)
                    : ArmActuatorSlot.Hand;
                if (limit.HasFlag(ArmActuatorSlot.PartLower))
                    add_default(ArmActuatorSlot.PartLower);
                if (limit.HasFlag(ArmActuatorSlot.PartHand))
                    add_default(ArmActuatorSlot.PartHand);
            }
        }

        public void OnInstalled(WorkOrderEntry_InstallComponent order, SimGameState state, MechDef mech)
        {
            try
            {
                    if (order.DesiredLocation != ChassisLocations.None &&
                        ChassisLocations.Arms.HasFlag(order.DesiredLocation))
                    {
                        var slots = ArmActuatorCC.ClearDefaultActuators(mech, order.DesiredLocation);
                        CustomComponents.Control.LogDebug(DType.ComponentInstall,
                            $"- ArmActuator removing, left {slots}");

                        ArmActuatorCC.AddDefaultToInventory(mech, state, order.DesiredLocation,
                            ArmActuatorSlot.PartShoulder, ref slots);
                        ArmActuatorCC.AddDefaultToInventory(mech, state, order.DesiredLocation,
                            ArmActuatorSlot.PartUpper, ref slots);
                        CustomComponents.Control.LogDebug(DType.ComponentInstall, $"-- done");

                        if (!ArmActuatorCC.IsIgnoreFullActuators(mech))
                        {
                            var limit = mech.Chassis.Is<ArmActuatorSupport>(out var s)
                                ? s.GetLimit(order.DesiredLocation)
                                : ArmActuatorSlot.Hand;
                            if (limit.HasFlag(ArmActuatorSlot.PartLower))
                                ArmActuatorCC.AddDefaultToInventory(mech, state, order.DesiredLocation,
                                    ArmActuatorSlot.PartLower, ref slots);
                            if (limit.HasFlag(ArmActuatorSlot.PartHand))
                                ArmActuatorCC.AddDefaultToInventory(mech, state, order.DesiredLocation,
                                    ArmActuatorSlot.PartHand, ref slots);
                        }
                    }

                    if (order.PreviousLocation != ChassisLocations.None &&
                        ChassisLocations.Arms.HasFlag(order.PreviousLocation))
                    {
                        var slots = ArmActuatorCC.ClearDefaultActuators(mech, order.PreviousLocation);
                        CustomComponents.Control.LogDebug(DType.ComponentInstall,
                            $"- ArmActuator adding, left {slots}");

                        ArmActuatorCC.AddDefaultToInventory(mech, state, order.PreviousLocation,
                            ArmActuatorSlot.PartShoulder, ref slots);
                        ArmActuatorCC.AddDefaultToInventory(mech, state, order.PreviousLocation,
                            ArmActuatorSlot.PartUpper, ref slots);
                        CustomComponents.Control.LogDebug(DType.ComponentInstall, $"-- done");

                        if (!ArmActuatorCC.IsIgnoreFullActuators(mech))
                        {
                            var limit = mech.Chassis.Is<ArmActuatorSupport>(out var s)
                                ? s.GetLimit(order.PreviousLocation)
                                : ArmActuatorSlot.Hand;
                            if (limit.HasFlag(ArmActuatorSlot.PartLower))
                                ArmActuatorCC.AddDefaultToInventory(mech, state, order.PreviousLocation,
                                    ArmActuatorSlot.PartLower, ref slots);
                            if (limit.HasFlag(ArmActuatorSlot.PartHand))
                                ArmActuatorCC.AddDefaultToInventory(mech, state, order.PreviousLocation,
                                    ArmActuatorSlot.PartHand, ref slots);
                        }
                    }
            }
            catch (Exception e)
            {
                CustomComponents.Control.LogError(e);
            }
        }
    }
}