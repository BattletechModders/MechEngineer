using System;
using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using CustomComponents.ExtendedDetails;
using MechEngineer.Features.OverrideDescriptions;

namespace MechEngineer.Features.ArmActuators
{
    [Flags]
    public enum ArmActuatorSlot
    {
        None = 0,
        
        PartShoulder = 1 << 0,
        PartUpper = 1 << 1,
        PartLower = 1 << 2,
        PartHand = 1 << 3,

        Upper = PartShoulder | PartUpper,
        Lower = Upper | PartLower,
        Hand = Lower | PartHand,
    }

    [CustomComponent("ArmActuatorSupport")]
    public class ArmActuatorSupport : SimpleCustomChassis, IAfterLoad
    {
        public ArmActuatorSlot LeftLimit = ArmActuatorSlot.Hand;
        public ArmActuatorSlot RightLimit = ArmActuatorSlot.Hand;

        public string LeftDefaultShoulder = null;
        public string RightDefaultShoulder = null;
        public string LeftDefaultUpper = null;
        public string RightDefaultUpper = null;
        public string LeftDefaultLower = null;
        public string RightDefaultLower = null;
        public string LeftDefaultHand = null;
        public string RightDefaultHand = null;

        public bool IgnoreFullActuators = false;

        public void OnLoaded(Dictionary<string, object> values)
        {
            var descriptions = new string[]
            {
                $"Left arm actuator support: {LeftLimit}",
                $"Right arm actuator support: {RightLimit}",
            };
            Control.mod.Logger.Log($"yep loading actuator support descriptions for {Def.Description.Id}");
            BonusDescriptions.AddTemplatedExtendedDetail(
                Def.GetOrCreate(() => new ExtendedDetails(Def.Description)),
                descriptions,
                OverrideDescriptionsFeature.settings.BonusDescriptionsElementTemplate,
                OverrideDescriptionsFeature.settings.BonusDescriptionsDescriptionTemplate,
                OverrideDescriptionsFeature.settings.DescriptionIdentifier
            );
        }

        public ArmActuatorSlot GetLimit(ChassisLocations location)
        {
            if (location == ChassisLocations.LeftArm)
                return LeftLimit;
            if (location == ChassisLocations.RightArm)
                return RightLimit;
            return ArmActuatorSlot.PartHand;
        }

        public string GetShoulder(ChassisLocations location)
        {
            if (location == ChassisLocations.LeftArm)
                return LeftDefaultShoulder;
            if (location == ChassisLocations.RightArm)
                return RightDefaultShoulder;
            return null;
        }

        public string GetUpper(ChassisLocations location)
        {
            if (location == ChassisLocations.LeftArm)
                return LeftDefaultUpper;
            if (location == ChassisLocations.RightArm)
                return RightDefaultUpper;
            return null;
        }

        public string GetLower(ChassisLocations location)
        {
            if (location == ChassisLocations.LeftArm)
                return LeftDefaultLower;
            if (location == ChassisLocations.RightArm)
                return RightDefaultLower;
            return null;
        }

        public string GetHand(ChassisLocations location)
        {
            if (location == ChassisLocations.LeftArm)
                return LeftDefaultHand;
            if (location == ChassisLocations.RightArm)
                return RightDefaultHand;
            return null;
        }
    }
}