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
                    return "";
            }
        }

        public static string GetDefaultActuator(MechDef mech, ChassisLocations location, ArmActuatorSlot slot)
        {
            if (location != ChassisLocations.RightArm && location != ChassisLocations.LeftArm)
                return "";

            if (mech?.Chassis == null || !mech.Chassis.Is<ArmSupportCBT>(out var support))
            {
                if (location == ChassisLocations.RightArm || location == ChassisLocations.LeftArm)
                    return GetComponentIdForSlot(slot);
                return "";
            }

            var part = support.GetByLocation(location);
            if (part == null)
                return GetComponentIdForSlot(slot);

            if (slot == ArmActuatorSlot.Shoulder)
                return part.DefaultShoulder;
            if (slot == ArmActuatorSlot.Upper)
                return part.DefaultUpper;
            return "";
        }

        internal static void ClearInventory(MechDef mech, List<MechComponentRef> result, SimGameState state)
        {
            ArmActuatorSlot total_slot = ArmActuatorSlot.None;

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

                if (add_item(GetDefaultActuator(mech, location, slot)))
                    return;
                add_item(GetComponentIdForSlot(slot));
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
            return false;
        }
    }
}