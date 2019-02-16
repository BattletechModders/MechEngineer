using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using HBS.Extensions;
using Localize;

namespace MechEngineer
{
    public static class ArmActuatorCBTHandler
    {
        private static string GetComponentIdForSlot(ArmActuatorSlot slot)
        {
            switch (slot)
            {
                case ArmActuatorSlot.Shoulder:
                    return Control.settings.DefaultCBTShoulder;
                case ArmActuatorSlot.Upper:
                    return Control.settings.DefaultCBTUpper;
                case ArmActuatorSlot.Lower:
                    return Control.settings.DefaultCBTLower;
                case ArmActuatorSlot.Hand:
                    return Control.settings.DefaultCBTHand;
                default:
                    return null;
            }
        }

        public static string GetDefaultActuator(MechDef mech, ChassisLocations location, ArmActuatorSlot slot)
        {
            if (location != ChassisLocations.RightArm && location != ChassisLocations.LeftArm)
            {
                return null;
            }

            if (!mech.Chassis.Is<ArmSupportCBT>(out var support))
            {
                return GetComponentIdForSlot(slot);
            }

            var part = support.GetByLocation(location);
            if (part == null)
                return GetComponentIdForSlot(slot);

            switch (slot)
            {
                case ArmActuatorSlot.Shoulder:
                    return part.DefaultShoulder;
                case ArmActuatorSlot.Upper:
                    return part.DefaultUpper;
                default:
                    return null;
            }
        }

        internal static void ClearInventory(MechDef mech, List<MechComponentRef> result, SimGameState state)
        {
            var total_slot = ArmActuatorSlot.None;

            void add_default(ChassisLocations location, ArmActuatorSlot slot)
            {
                bool add_item(string id)
                {
                    if (string.IsNullOrEmpty(id))
                        return false;

                    var r = DefaultHelper.CreateRef(id, ComponentType.Upgrade, state.DataManager, state);
                    if (!r.Is<ArmActuatorCBT>(out var actuator) && (actuator.Slot & total_slot) == 0)
                    {
                        r.SetData(location, -1, ComponentDamageLevel.Functional, true);
                        result.Add(r);
                        total_slot = total_slot | actuator.Slot;
                        return true;
                    }

                    return false;
                }

                if (total_slot.HasFlag(slot))
                    return;

                add_item(GetDefaultActuator(mech, location, slot));
            }

            void clear_side(ChassisLocations location)
            {
                result.RemoveAll(i => i.Is<ArmActuator>() && !i.IsModuleFixed(mech));
                total_slot = ArmActuatorSlot.None;
                foreach (var item in result)
                {
                    if (item.Is<ArmActuatorCBT>(out var a))
                        total_slot = total_slot | a.Slot;
                }
                add_default(location, ArmActuatorSlot.Shoulder);
                add_default(location, ArmActuatorSlot.Upper);
            }

            clear_side(ChassisLocations.LeftArm);
            clear_side(ChassisLocations.RightArm);
        }

        public static void ValidateMech(Dictionary<MechValidationType, List<Text>> errors, MechValidationLevel validationlevel, MechDef mechdef)
        {
        }

        public static bool CanBeFielded(MechDef mechdef)
        {
            bool check_location(ChassisLocations location)
            {
                //occupied slots
                var slots = ArmActuatorSlot.None;

                //list of actuators in location
                var actuators = from item in mechdef.Inventory
                    where item.MountedLocation == location &&
                          item.Is<ArmActuatorCBT>()
                    select item.GetComponent<ArmActuatorCBT>();


                //get max avaliable actuator
                ArmActuatorSlot max = ArmActuatorSlot.Hand;

                if (mechdef.Chassis.Is<ArmSupportCBT>(out var support))
                {
                    var part = support.GetByLocation(location);
                    if (part != null)
                        max = part.MaxActuator;
                }

                foreach (var actuator in actuators)
                {
                    // if more then 1 actuator occupy 1 slot
                    if ((slots & actuator.Slot) != 0)
                        return false;

                    //correcting max slot if actuator has limits
                    if (max > actuator.MaxSlot)
                        max = actuator.MaxSlot;

                    //save actuator to slots
                    slots = slots | actuator.Slot;
                }

                // if not support hand/lower
                if (slots > max)
                    return false;
                
                //if not have shoulder
                if (!slots.HasFlag(ArmActuatorSlot.Shoulder))
                    return false;

                //if not have upper
                if (!slots.HasFlag(ArmActuatorSlot.Upper))
                    return false;

                //if have hand but not lower
                if (slots.HasFlag(ArmActuatorSlot.Hand) && !slots.HasFlag(ArmActuatorSlot.Lower))
                    return false;

                return true;
            }

            return check_location(ChassisLocations.LeftArm) && check_location(ChassisLocations.RightArm);

        }
    }
}