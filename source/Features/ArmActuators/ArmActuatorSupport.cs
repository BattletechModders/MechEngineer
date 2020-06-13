using System;
using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using CustomComponents.ExtendedDetails;
using Harmony;

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

        public string CustomDescription = null;

        public string LeftDefaultShoulder = null;
        public string RightDefaultShoulder = null;
        public string LeftDefaultUpper = null;
        public string RightDefaultUpper = null;
        public string LeftDefaultLower = null;
        public string RightDefaultLower = null;
        public string LeftDefaultHand = null;
        public string RightDefaultHand = null;

        public bool IgnoreFullActuators = false;

        private string Description
        {
            get
            {
                if (CustomDescription != null)
                {
                    return string.Format(ArmActuatorFeature.Shared.Settings.CustomDescriptionTemplate, CustomDescription);
                }
                else if (LeftLimit < ArmActuatorSlot.Hand || RightLimit < ArmActuatorSlot.Hand)
                {
                    var left = LeftLimit.ToString().ToLowerInvariant();
                    var right = RightLimit.ToString().ToLowerInvariant();
                    return string.Format(ArmActuatorFeature.Shared.Settings.DescriptionTemplate, left, right);
                }
                return null;
            }
        }

        public void OnLoaded(Dictionary<string, object> values)
        {
            var text = Description;
            if (text == null)
            {
                return;
            }

            var extended = ExtendedDetails.GetOrCreate(Def);
            var detail = new ExtendedDetail
            {
                Index = -1,
                Text = Description,
                Identifier = ArmActuatorFeature.Shared.Settings.DescriptionIdentifier
            };
            extended.AddDetail(detail);

            // sync YangsThoughts
            Traverse.Create(Def)
                .Property<string>(nameof(Def.YangsThoughts))
                .Value = Def.Description.Details;
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