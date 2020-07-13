using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using HBS.Extensions;
using Localize;
using MechEngineer.Features.DynamicSlots;

namespace MechEngineer.Features.ArmActuators
{
    internal class ArmActuatorCC
    {
        private static string GetComponentIdForSlot(ArmActuatorSlot slot)
        {
            switch (slot)
            {
                case ArmActuatorSlot.PartShoulder:
                    return ArmActuatorFeature.settings.DefaultCBTShoulder;
                case ArmActuatorSlot.PartUpper:
                    return ArmActuatorFeature.settings.DefaultCBTUpper;
                case ArmActuatorSlot.PartLower:
                    return ArmActuatorFeature.settings.DefaultCBTDefLower;
                case ArmActuatorSlot.PartHand:
                    return ArmActuatorFeature.settings.DefaultCBTDefHand;
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

            if (mech == null || !mech.Chassis.Is<ArmActuatorSupport>(out var support))
            {
                return GetComponentIdForSlot(slot);
            }

            switch (slot)
            {
                case ArmActuatorSlot.PartShoulder:
                    return support.GetShoulder(location);
                case ArmActuatorSlot.PartUpper:
                    return support.GetUpper(location);
                case ArmActuatorSlot.PartLower:
                    return support.GetLower(location);
                case ArmActuatorSlot.PartHand:
                    return support.GetHand(location);
                default:
                    return null;
            }
        }

        internal static void ClearInventory(MechDef mech, List<MechComponentRef> result, SimGameState state)
        {
            if (SkipArmActuatorValidationAndFixingForMechDef(mech))
            {
                return;
            }

            var total_slot = ArmActuatorSlot.None;

            void add_default(ChassisLocations location, ArmActuatorSlot slot)
            {
                bool add_item(string id)
                {
                    CustomComponents.Control.LogDebug(DType.ClearInventory, $"--- adding {id} to {location}");
                    if (string.IsNullOrEmpty(id))
                    {
                        CustomComponents.Control.LogDebug(DType.ClearInventory, $"---- is null - skipping");
                        return false;
                    }

                    var r = DefaultHelper.CreateRef(id, ComponentType.Upgrade, state.DataManager, state);

                    if (r.Is<ArmActuator>(out var actuator) && (actuator.Type & total_slot) == 0)
                    {
                        r.SetData(location, -1, ComponentDamageLevel.Functional, true);
                        result.Add(r);
                        total_slot = total_slot | actuator.Type;
                        return true;
                    }

                    return false;
                }

                if (total_slot.HasFlag(slot))
                {
                    CustomComponents.Control.LogDebug(DType.ClearInventory, $"--- already have {slot} skip");
                    return;
                }

                if (!add_item(GetDefaultActuator(mech, location, slot)))
                    add_item(GetDefaultActuator(null, location, slot));
            }

            void clear_side(ChassisLocations location)
            {
                result.RemoveAll(i => i.MountedLocation == location & i.Is<ArmActuator>() && !i.IsModuleFixed(mech));
                total_slot = ArmActuatorSlot.None;
                foreach (var item in result.Where(i => i.MountedLocation == location))
                {
                    if (item.Is<ArmActuator>(out var a))
                        total_slot = total_slot | a.Type;
                }
                CustomComponents.Control.LogDebug(DType.ClearInventory, $"-- {location} current {total_slot}");
                add_default(location, ArmActuatorSlot.PartShoulder);
                add_default(location, ArmActuatorSlot.PartUpper);

                if (ArmActuatorFeature.settings.ForceFullDefaultActuators && !IsIgnoreFullActuators(mech))
                {
                    var limit = mech.Chassis.Is<ArmActuatorSupport>(out var s)
                        ? s.GetLimit(location)
                        : ArmActuatorSlot.Hand;

                    if(limit.HasFlag(ArmActuatorSlot.Lower))
                        add_default(location, ArmActuatorSlot.PartLower);
                    if (limit.HasFlag(ArmActuatorSlot.Hand))
                        add_default(location, ArmActuatorSlot.PartHand);
                }
            }



            clear_side(ChassisLocations.LeftArm);
            clear_side(ChassisLocations.RightArm);
        }

        public static void ValidateMech(Dictionary<MechValidationType, List<Text>> errors, MechValidationLevel validationlevel, MechDef mechdef)
        {
            if (SkipArmActuatorValidationAndFixingForMechDef(mechdef))
            {
                return;
            }

            void check_location(ChassisLocations location)
            {
                //occupied slots
                var slots = ArmActuatorSlot.None;

                //list of actuators in location
                var actuators = from item in mechdef.Inventory
                    where item.MountedLocation == location &&
                            item.Is<ArmActuator>()
                    select item.GetComponent<ArmActuator>();


                //get max avaliable actuator
                ArmActuatorSlot max = mechdef.Chassis.Is<ArmActuatorSupport>(out var support)
                    ? support.GetLimit(location)
                    : ArmActuatorSlot.PartHand;

                foreach (var actuator in actuators)
                {
                    // if more then 1 actuator occupy 1 slot
                    if ((slots & actuator.Type) != 0)
                    {
                        errors[MechValidationType.InvalidInventorySlots]
                            .Add(new Text($"{location} have more then one {actuator.Type} actuator"));
                    }

                    //correcting max slot if actuator has limits
                    if (max > actuator.MaxSlot)
                        max = actuator.MaxSlot;

                    //save actuator to slots
                    slots = slots | actuator.Type;
                }

                if (ArmActuatorFeature.settings.ExtendHandLimit)
                {
                    if (max == ArmActuatorSlot.PartHand)
                        max = ArmActuatorSlot.Hand;

                    if (max == ArmActuatorSlot.PartUpper)
                        max = ArmActuatorSlot.Upper;

                    if (max == ArmActuatorSlot.PartLower)
                        max = ArmActuatorSlot.Lower;
                }

                // if not support hand/lower
                if (slots > max)
                    errors[MechValidationType.InvalidInventorySlots]
                        .Add(new Text($"{location} cannot support more then {max} actuator"));

                //if not have shoulder
                if (!slots.HasFlag(ArmActuatorSlot.PartShoulder))
                    errors[MechValidationType.InvalidInventorySlots].Add(new Text($"{location} missing Shoulder"));

                //if not have upper
                if (!slots.HasFlag(ArmActuatorSlot.PartUpper))
                    errors[MechValidationType.InvalidInventorySlots].Add(new Text($"{location} missing Upper Arm"));

                //if have hand but not lower
                if (slots.HasFlag(ArmActuatorSlot.PartHand) && !slots.HasFlag(ArmActuatorSlot.PartLower))
                    errors[MechValidationType.InvalidInventorySlots].Add(new Text($"{location} missing Lower Arm"));
            }

            check_location(ChassisLocations.LeftArm);
            check_location(ChassisLocations.RightArm);
        }

        public static bool CanBeFielded(MechDef mechdef)
        {
            if (SkipArmActuatorValidationAndFixingForMechDef(mechdef))
            {
                return true;
            }

            bool check_location(ChassisLocations location)
            {
                //occupied slots
                var slots = ArmActuatorSlot.None;

                //list of actuators in location
                var actuators = from item in mechdef.Inventory
                    where item.MountedLocation == location &&
                            item.Is<ArmActuator>()
                    select item.GetComponent<ArmActuator>();



                //get max avaliable actuator
                ArmActuatorSlot max = mechdef.Chassis.Is<ArmActuatorSupport>(out var support)
                    ? support.GetLimit(location)
                    : ArmActuatorSlot.PartHand;


                foreach (var actuator in actuators)
                {
                    // if more then 1 actuator occupy 1 slot
                    if ((slots & actuator.Type) != 0)
                        return false;

                    //correcting max slot if actuator has limits
                    if (max > actuator.MaxSlot)
                        max = actuator.MaxSlot;

                    //save actuator to slots
                    slots = slots | actuator.Type;
                }

                if (ArmActuatorFeature.settings.ExtendHandLimit)
                {
                    if (max == ArmActuatorSlot.PartHand)
                        max = ArmActuatorSlot.Hand;

                    if (max == ArmActuatorSlot.PartUpper)
                        max = ArmActuatorSlot.Upper;

                    if (max == ArmActuatorSlot.PartLower)
                        max = ArmActuatorSlot.Lower;
                }

                // if not support hand/lower
                if (slots > max)
                    return false;

                //if not have shoulder
                if (!slots.HasFlag(ArmActuatorSlot.PartShoulder))
                    return false;

                //if not have upper
                if (!slots.HasFlag(ArmActuatorSlot.PartUpper))
                    return false;

                //if have hand but not lower
                if (slots.HasFlag(ArmActuatorSlot.PartHand) && !slots.HasFlag(ArmActuatorSlot.PartLower))
                    return false;

                return true;
            }

            return check_location(ChassisLocations.LeftArm) && check_location(ChassisLocations.RightArm);
        }

        public static void FixCBTActuators(List<MechDef> mechdefs, SimGameState simgame)
        {
            if (simgame == null)
            {
                foreach (var mechdef in mechdefs)
                {
                    add_full_actuators(mechdef);
                }
            }
            else
            {
                foreach (var mechdef in mechdefs)
                {
                    add_default_actuators(mechdef, simgame);
                }
            }
        }

        private static void add_default_actuators(MechDef mechdef, SimGameState simgame)
        {
            void process_location(ChassisLocations location)
            {
                var total_slots = ArmActuatorSlot.None;

                foreach (var item in mechdef.Inventory.Where(i => i.MountedLocation == location && i.Is<ArmActuator>()).Select(i => i.GetComponent<ArmActuator>()))
                {
                    total_slots = total_slots | item.Type;
                }

                AddDefaultToInventory(mechdef, simgame, location, ArmActuatorSlot.PartShoulder, ref total_slots);
                AddDefaultToInventory(mechdef, simgame, location, ArmActuatorSlot.PartUpper, ref total_slots);
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

                if (r.Is<ArmActuator>(out var actuator) && (actuator.Type & total_slot) == 0)
                {
                    DefaultHelper.AddInventory(id, mechdef, location, ComponentType.Upgrade, simgame);
                    total_slot = total_slot | actuator.Type;
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
                !(i.MountedLocation == location && i.Is<ArmActuator>() && i.IsFixed &&
                  !i.IsModuleFixed(mechdef))).ToArray());

            var slot = ArmActuatorSlot.None;
            foreach (var item in mechdef.Inventory.Where(i => i.MountedLocation == location && i.Is<ArmActuator>()))
            {
                var actuator = item.GetComponent<ArmActuator>();
                slot = slot | actuator.Type;
            }

            return slot;
        }

        private static void add_full_actuators(MechDef mechdef)
        {
            void process_location(ChassisLocations location)
            {
                var total_slots = mechdef.Inventory.Where(i => i.MountedLocation == location && i.Is<ArmActuator>())
                    .Select(i => i.GetComponent<ArmActuator>())
                    .Aggregate(ArmActuatorSlot.None, (current, item) => current | item.Type);

                //if not present any actuators
                if (total_slots == ArmActuatorSlot.None)
                {
                    //add shoulder, and upper
                    AddDefaultToInventory(mechdef, null, location, ArmActuatorSlot.PartShoulder, ref total_slots);
                    AddDefaultToInventory(mechdef, null, location, ArmActuatorSlot.PartUpper, ref total_slots);

                    //get max avaliable actuator
                    ArmActuatorSlot max = mechdef.Chassis.Is<ArmActuatorSupport>(out var support) ?
                        support.GetLimit(location) :
                        ArmActuatorSlot.PartHand;

                    foreach (var item in mechdef.Inventory.Where(i => i.MountedLocation == location &&
                                                                      i.Is<ArmActuator>()).Select(i => i.GetComponent<ArmActuator>()))
                    {
                        if (item.MaxSlot < max)
                            max = item.MaxSlot;
                    }

                    var builder = new MechDefBuilder(mechdef);
                    if (max >= ArmActuatorSlot.PartLower && !total_slots.HasFlag(ArmActuatorSlot.PartLower))
                    {
                        var def = UnityGameInstance.BattleTechGame.DataManager.UpgradeDefs.Get(ArmActuatorFeature.settings.DefaultCBTLower);
                        if (def == null)
                        {
                            return;
                        }
                        if (builder.Add(def, location) == null)
                        {
                            return;
                        }
                    }

                    if (max >= ArmActuatorSlot.PartHand && !total_slots.HasFlag(ArmActuatorSlot.PartHand))
                    {
                        var def = UnityGameInstance.BattleTechGame.DataManager.UpgradeDefs.Get(ArmActuatorFeature.settings.DefaultCBTHand);
                        builder.Add(def, location);
                    }
                    mechdef.SetInventory(builder.Inventory.ToArray());
                }
                else
                {
                    //recheck and add if needed shoulder and arm
                    AddDefaultToInventory(mechdef, null, location, ArmActuatorSlot.PartShoulder, ref total_slots);
                    AddDefaultToInventory(mechdef, null, location, ArmActuatorSlot.PartUpper, ref total_slots);
                }
            }

            process_location(ChassisLocations.RightArm);
            process_location(ChassisLocations.LeftArm);
        }

        public static bool IsIgnoreFullActuators(MechDef mech)
        {
            if (!ArmActuatorFeature.settings.ForceFullDefaultActuators)
                return true;

            if (mech.Chassis.Is<ArmActuatorSupport>(out var s) && s.IgnoreFullActuators)
                return true;

            if (!string.IsNullOrEmpty(ArmActuatorFeature.settings.IgnoreFullActuatorsTag) && mech.Chassis.ChassisTags.Contains(ArmActuatorFeature.settings.IgnoreFullActuatorsTag))
                return true;

            return false;
        }

        static bool SkipArmActuatorValidationAndFixingForMechDef(MechDef mechDef)
        {
            return mechDef.IgnoreAutofix();
        }


        public static void ValidateMechFF(Dictionary<MechValidationType, List<Text>> errors, MechValidationLevel validationlevel, MechDef mechdef)
        {
            if (SkipArmActuatorValidationAndFixingForMechDef(mechdef))
            {
                return;
            }

            if (IsIgnoreFullActuators(mechdef))
            {
                ValidateMech(errors, validationlevel, mechdef);
                return;
            }

            void check_side(ChassisLocations location, ArmActuatorSlot limit)
            {
                var total_slots = ArmActuatorSlot.None;

                foreach (var item in mechdef.Inventory.Where(i => i.MountedLocation == location && i.Is<ArmActuator>()))
                {
                    var a = item.GetComponent<ArmActuator>();
                    if ((a.Type & total_slots) != 0)
                        errors[MechValidationType.InvalidInventorySlots].Add(new Text($"{location} have more then one {a.Type} actuator"));
                    total_slots = total_slots | a.Type;
                }

                if (total_slots < limit)
                    errors[MechValidationType.InvalidInventorySlots].Add(new Text($"{location} dont have {(ArmActuatorSlot)(limit - total_slots):G} actuator"));
                else if (total_slots > limit)
                    errors[MechValidationType.InvalidInventorySlots].Add(new Text($"{location} dont support {(ArmActuatorSlot)(total_slots - limit):G} actuator"));

            }

            ArmActuatorSlot left = ArmActuatorSlot.Hand;
            ArmActuatorSlot right = ArmActuatorSlot.Hand;
            if (mechdef.Chassis.Is<ArmActuatorSupport>(out var support))
            {
                left = support.LeftLimit;
                right = support.RightLimit;
            }

            check_side(ChassisLocations.LeftArm, left);
            check_side(ChassisLocations.RightArm, right);
        }

        public static bool CanBeFieldedFF(MechDef mechdef)
        {
            if (SkipArmActuatorValidationAndFixingForMechDef(mechdef))
            {
                return true;
            }

            if (IsIgnoreFullActuators(mechdef))
                return CanBeFielded(mechdef);

            bool check_side(ChassisLocations location, ArmActuatorSlot limit)
            {
                var total_slots = ArmActuatorSlot.None;

                foreach (var item in mechdef.Inventory.Where(i => i.MountedLocation == location && i.Is<ArmActuator>()))
                {
                    var a = item.GetComponent<ArmActuator>();
                    if ((a.Type & total_slots) != 0)
                        return false;
                    total_slots = total_slots | a.Type;
                }

                if (total_slots < limit)
                    return false;
                if (total_slots > limit)
                    return false;
                return true;
            }

            ArmActuatorSlot left = ArmActuatorSlot.Hand;
            ArmActuatorSlot right = ArmActuatorSlot.Hand;
            if (mechdef.Chassis.Is<ArmActuatorSupport>(out var support))
            {
                left = support.LeftLimit;
                right = support.RightLimit;
            }

            return check_side(ChassisLocations.LeftArm, left) && check_side(ChassisLocations.RightArm, right);

        }

        public static void FixCBTActuatorsFF(List<MechDef> mechdefs, SimGameState simgame)
        {
            void fix_hand(MechDef mechDef, ChassisLocations location)
            {
                var total_slots = ArmActuatorSlot.None;
                foreach (var item in mechDef.Inventory.Where(i => i.Is<ArmActuator>() && i.MountedLocation == location)
                    .Select(i => i.GetComponent<ArmActuator>()))
                    total_slots = total_slots | item.Type;

                var limit = mechDef.Chassis.Is<ArmActuatorSupport>(out var s)
                    ? s.GetLimit(location)
                    : ArmActuatorSlot.Hand;
                AddDefaultToInventory(mechDef, simgame, location, ArmActuatorSlot.PartShoulder, ref total_slots);
                AddDefaultToInventory(mechDef, simgame, location, ArmActuatorSlot.PartUpper, ref total_slots);
                if (limit.HasFlag(ArmActuatorSlot.PartLower))
                    AddDefaultToInventory(mechDef, simgame, location, ArmActuatorSlot.PartLower, ref total_slots);
                if (limit.HasFlag(ArmActuatorSlot.PartHand))
                    AddDefaultToInventory(mechDef, simgame, location, ArmActuatorSlot.PartHand, ref total_slots);
            }

            foreach (var mechdef in mechdefs)
            {
                if (IsIgnoreFullActuators(mechdef))
                {
                    FixCBTActuators(mechdefs, simgame);
                    continue;
                }

                fix_hand(mechdef, ChassisLocations.LeftArm);
                fix_hand(mechdef, ChassisLocations.RightArm);
            }
        }
    }
}