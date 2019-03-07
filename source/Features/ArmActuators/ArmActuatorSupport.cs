﻿using System;
using BattleTech;
using CustomComponents;

namespace MechEngineer
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
    public class ArmActuatorSupport : SimpleCustomChassis
    {
        public ArmActuatorSlot LeftLimit = ArmActuatorSlot.Hand;
        public ArmActuatorSlot RightLimit = ArmActuatorSlot.Hand;

        public string LeftDefaultShoulder = null;
        public string RightDefaultShoulder = null;
        public string LeftDefaultUpper = null;
        public string RightDefaultUpper = null;


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