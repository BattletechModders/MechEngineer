using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    [Flags]
    public enum ArmActuatorSlot
    {
        None = 0,
        Shoulder = 1,
        Upper = 2,
        Lower = 4,
        Hand = 8,

        FullHand = 15,
        FullLower = 7,
        FullUpper = 3
    }

    [CustomComponent("ArmActuatorSupport")]
    public class ArmSupportCBT : CustomComponents.ChassisCusomComponent
    {
        public ArmActuatorSlot LeftLimit = ArmActuatorSlot.FullHand;
        public ArmActuatorSlot RightLimit = ArmActuatorSlot.FullHand;

        public string LeftDefaultShoulder = "";
        public string RightDefaultShoulder = "";
        public string LeftDefaultUpper = "";
        public string RightDefaultUpper = "";


        public ArmActuatorSlot GetLimit(ChassisLocations location)
        {
            if (location == ChassisLocations.LeftArm)
                return LeftLimit;
            if (location == ChassisLocations.RightArm)
                return RightLimit;
            return ArmActuatorSlot.Hand;
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
    }
}