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
        
        PartShoulder = 1,
        PartUpper = 2,
        PartLower = 4,
        PartHand = 8,

        Upper = PartShoulder | PartUpper,
        Lower = Upper | PartLower,
        Hand = Lower | PartHand,
    }

    [CustomComponent("ArmActuatorSupport")]
    public class ArmActuatorSupport : ChassisCustom
    {
        public ArmActuatorSlot LeftLimit = ArmActuatorSlot.Hand;
        public ArmActuatorSlot RightLimit = ArmActuatorSlot.Hand;

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
    }
}