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
            
            if (mech == null || !mech.Chassis.Is<ArmSupportCBT>(out var support))
            {
                return GetComponentIdForSlot(slot);
            }

            switch (slot)
            {
                case ArmActuatorSlot.Shoulder:
                    return support.GetShoulder(location);
                case ArmActuatorSlot.Upper:
                    return support.GetUpper(location);
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
            void check_location(ChassisLocations location)
            {
                //occupied slots
                var slots = ArmActuatorSlot.None;

                //list of actuators in location
                var actuators = from item in mechdef.Inventory
                                where item.MountedLocation == location &&
                                      item.Is<ArmActuatorCBT>()
                                select item.GetComponent<ArmActuatorCBT>();


                //get max avaliable actuator
                ArmActuatorSlot max = mechdef.Chassis.Is<ArmSupportCBT>(out var support)  ? 
                    support.GetLimit(location) :
                    ArmActuatorSlot.Hand;

                foreach (var actuator in actuators)
                {
                    // if more then 1 actuator occupy 1 slot
                    if ((slots & actuator.Slot) != 0)
                    {
                        errors[MechValidationType.InvalidInventorySlots].Add(new Text($"{location} have more then one {actuator.Slot} actuator"));
                    }

                    //correcting max slot if actuator has limits
                    if (max > actuator.MaxSlot)
                        max = actuator.MaxSlot;

                    //save actuator to slots
                    slots = slots | actuator.Slot;
                }

                if (Control.settings.ExtendHandLimit)
                {
                    if (max == ArmActuatorSlot.Hand)
                        max = ArmActuatorSlot.FullHand;

                    if (max == ArmActuatorSlot.Upper)
                        max = ArmActuatorSlot.FullUpper;

                    if (max == ArmActuatorSlot.Lower)
                        max = ArmActuatorSlot.FullLower;
                }

                // if not support hand/lower
                if (slots > max)
                    errors[MechValidationType.InvalidInventorySlots].Add(new Text($"{location} cannot support more then {max} actuator"));

                //if not have shoulder
                if (!slots.HasFlag(ArmActuatorSlot.Shoulder))
                    errors[MechValidationType.InvalidInventorySlots].Add(new Text($"{location} missing Shoulder"));

                //if not have upper
                if (!slots.HasFlag(ArmActuatorSlot.Upper))
                    errors[MechValidationType.InvalidInventorySlots].Add(new Text($"{location} missing Upper Arm"));

                //if have hand but not lower
                if (slots.HasFlag(ArmActuatorSlot.Hand) && !slots.HasFlag(ArmActuatorSlot.Lower))
                    errors[MechValidationType.InvalidInventorySlots].Add(new Text($"{location} missing Lower Arm"));
            }

            check_location(ChassisLocations.LeftArm);
            check_location(ChassisLocations.RightArm);

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
                ArmActuatorSlot max = mechdef.Chassis.Is<ArmSupportCBT>(out var support) ?
                    support.GetLimit(location) :
                    ArmActuatorSlot.Hand;


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

                if (Control.settings.ExtendHandLimit)
                {
                    if (max == ArmActuatorSlot.Hand)
                        max = ArmActuatorSlot.FullHand;

                    if (max == ArmActuatorSlot.Upper)
                        max = ArmActuatorSlot.FullUpper;

                    if (max == ArmActuatorSlot.Lower)
                        max = ArmActuatorSlot.FullLower;
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

        public static void FixCBTActuators(List<MechDef> mechdefs, SimGameState simgame)
        {
            if (simgame == null)
                foreach (var mechdef in mechdefs)
                    add_full_actuators(mechdef);
            else
                foreach (var mechdef in mechdefs)
                    add_default_actuators(mechdef, simgame);
        }

        private static void add_default_actuators(MechDef mechdef, SimGameState simgame)
        {
            void process_location(ChassisLocations location)
            {
                var total_slots = ArmActuatorSlot.None;

                foreach (var item in mechdef.Inventory.Where(i => i.MountedLocation == location && i.Is<ArmActuatorCBT>()).Select(i => i.GetComponent<ArmActuatorCBT>()))
                {
                    total_slots = total_slots | item.Slot;
                }

                AddDefaultToInventory(mechdef, simgame, location, ArmActuatorSlot.Shoulder, ref total_slots);
                AddDefaultToInventory(mechdef, simgame, location, ArmActuatorSlot.Upper, ref total_slots);
            }

            process_location(ChassisLocations.RightArm);
            process_location(ChassisLocations.LeftArm);


        }

        internal static void AddDefaultToInventory(MechDef mechdef, SimGameState simgame, ChassisLocations location, ArmActuatorSlot slot, ref ArmActuatorSlot totalSlots)
        {
            bool add_item(string id, ref ArmActuatorSlot total_slot)
            {
                if (string.IsNullOrEmpty(id))
                    return false;

                var r = DefaultHelper.CreateRef(id, ComponentType.Upgrade, UnityGameInstance.BattleTechGame.DataManager, simgame);

                if (r.Is<ArmActuatorCBT>(out var actuator) && (actuator.Slot & total_slot) == 0)
                {
                    DefaultHelper.AddInventory(id, mechdef, location, ComponentType.Upgrade, simgame);
                    total_slot = total_slot | actuator.Slot;
                    return true;
                }

                return false;
            }

            CustomComponents.Control.LogDebug(DType.ComponentInstall, $"---- adding {slot} to {totalSlots}");
            if (totalSlots.HasFlag(slot))
            {
                CustomComponents.Control.LogDebug(DType.ComponentInstall, $"---- already present");
                return;
            }

            if (add_item(GetDefaultActuator(mechdef, location, slot), ref totalSlots))
                return;

            add_item(GetDefaultActuator(null, location, slot), ref totalSlots);
        }

        internal static ArmActuatorSlot ClearDefaultActuators(MechDef mechdef, ChassisLocations location)
        {
            mechdef.SetInventory(mechdef.Inventory.Where(i =>
                    i.MountedLocation == location && i.Is<ArmActuatorCBT>() && i.IsFixed &&
                    !i.IsModuleFixed(mechdef)).ToArray());

            var slot = ArmActuatorSlot.None;
            foreach (var item in mechdef.Inventory.Where(i => i.MountedLocation == location && i.Is<ArmActuatorCBT>()))
            {
                var actuator = item.GetComponent<ArmActuatorCBT>();
                slot = slot | actuator.Slot;
            }

            return slot;
        }

        private static void add_full_actuators(MechDef mechdef)
        {
            void process_location(ChassisLocations location)
            {
                var total_slots = mechdef.Inventory.Where(i => i.MountedLocation == location && i.Is<ArmActuatorCBT>())
                    .Select(i => i.GetComponent<ArmActuatorCBT>())
                    .Aggregate(ArmActuatorSlot.None, (current, item) => current | item.Slot);

                //if not present any actuators
                if (total_slots == ArmActuatorSlot.None)
                {
                    //add shoulder, and upper
                    AddDefaultToInventory(mechdef, null, location, ArmActuatorSlot.Shoulder, ref total_slots);
                    AddDefaultToInventory(mechdef, null, location, ArmActuatorSlot.Upper, ref total_slots);

                    //get max avaliable actuator
                    ArmActuatorSlot max = mechdef.Chassis.Is<ArmSupportCBT>(out var support) ?
                        support.GetLimit(location) :
                        ArmActuatorSlot.Hand;

                    foreach (var item in mechdef.Inventory.Where(i => i.MountedLocation == location &&
                        i.Is<ArmActuatorCBT>()).Select(i => i.GetComponent<ArmActuatorCBT>()))
                    {
                        if (item.MaxSlot < max)
                            max = item.MaxSlot;
                    }

                    if (max >= ArmActuatorSlot.Lower && !total_slots.HasFlag(ArmActuatorSlot.Lower))
                    {
                        var r = new MechComponentRef(Control.settings.DefaultCBTLower, null, ComponentType.Upgrade, location);
                        r.DataManager = UnityGameInstance.BattleTechGame.DataManager;
                        r.RefreshComponentDef();

                        var list = mechdef.Inventory.ToList();
                        list.Add(r);
                        mechdef.SetInventory(list.ToArray());
                    }

                    if (max >= ArmActuatorSlot.Hand && !total_slots.HasFlag(ArmActuatorSlot.Hand))
                    {
                        var r = new MechComponentRef(Control.settings.DefaultCBTHand, null, ComponentType.Upgrade, location);
                        r.DataManager = UnityGameInstance.BattleTechGame.DataManager;
                        r.RefreshComponentDef();

                        var list = mechdef.Inventory.ToList();
                        list.Add(r);
                        mechdef.SetInventory(list.ToArray());
                    }
                }
                else
                {
                    //recheck and add if needed shoulder and arm
                    AddDefaultToInventory(mechdef, null, location, ArmActuatorSlot.Shoulder, ref total_slots);
                    AddDefaultToInventory(mechdef, null, location, ArmActuatorSlot.Upper, ref total_slots);
                }


            }

            process_location(ChassisLocations.RightArm);
            process_location(ChassisLocations.LeftArm);
        }
    }
}