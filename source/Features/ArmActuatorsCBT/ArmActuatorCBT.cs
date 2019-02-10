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
        IAdjustDescription, IOnInstalled, IPostValidateDrop
    {
        public ArmActuatorSlot Slot { get; set; }
        public ArmActuatorSlot MaxSlot { get; set; } = ArmActuatorSlot.Hand;

        public string PreValidateDrop(MechLabItemSlotElement item, LocationHelper location, MechLabHelper mechlab)
        {
            var max = ArmActuatorSlot.Hand;

            if (mechlab.MechLab.activeMechDef.Chassis.Is<ArmSupportCBT>(out var support))
            {
                var part = support.GetByLocation(location.widget.loadout.Location);
                if (part != null && part.MaxActuator < Slot)
                {
                    return
                        $"Cannot install {item.ComponentRef.Def.Description.Name} mech support only up to {max} in {location.LocationName}";
                }
            }

            foreach (var slot in location.LocalInventory)
            {
                if (slot.ComponentRef.Is<ArmActuatorCBT>(out var actuator))
                {
                    if (actuator.MaxSlot < Slot)
                        return
                            $"Cannot install {item.ComponentRef.Def.Description.Name} {slot.ComponentRef.Def.Description.Name} limit {location.LocationName} to {actuator.MaxSlot}";

                    if (MaxSlot < actuator.Slot)
                        return
                        $"Cannot install {item.ComponentRef.Def.Description.Name} cose {actuator.Slot} present, remove it first";
                }
            }

            return string.Empty;
        }

        public string ReplaceValidateDrop(MechLabItemSlotElement drop_item, LocationHelper location, List<IChange> changes)
        {
            var slot = Slot;
            foreach (var item in location.LocalInventory.Where(i => i.ComponentRef.Is<ArmActuatorCBT>()))
            {

                var actuator = item.ComponentRef.GetComponent<ArmActuatorCBT>();
                if ((actuator.Slot & Slot) != 0)
                {
                    if (item.ComponentRef.IsModuleFixed(location.mechLab.activeMechDef))
                        return $"Cannot install {drop_item.ComponentRef.Def.Description.Name} - {Slot} occuped by {item.ComponentRef.Def.Description.Name}";
                    changes.Add(new RemoveChange(location.widget.loadout.Location, item));
                }
                else
                    slot = slot | actuator.Slot;
            }

            if (!slot.HasFlag(ArmActuatorSlot.Shoulder))
            {

            }
            if (!slot.HasFlag(ArmActuatorSlot.Upper))
            {

            }
            if (!slot.HasFlag(ArmActuatorSlot.Lower) && slot.HasFlag(ArmActuatorSlot.Hand))
            {

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
        }

        public void OnInstalled(WorkOrderEntry_InstallComponent order, SimGameState state, MechDef mech)
        {
        }

        public string PostValidateDrop(MechLabItemSlotElement drop_item, MechDef mech, List<InvItem> new_inventory, List<IChange> changes)
        {
            return string.Empty;
        }
    }
}