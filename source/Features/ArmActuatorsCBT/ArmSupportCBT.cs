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
        Hand = 8
    }

    [Serializable]
    public class SupportPart
    {
        public ArmActuatorSlot MaxActuator { get; set; } = ArmActuatorSlot.Hand;
        public string DefaultShoulder { get; set; }
        public string DefaultUpper { get; set; }
    }

    [CustomComponent("CBTArmActuatorSupport")]
    public class ArmSupportCBT : ChassisCusomComponent
    {
        public SupportPart Left { get; set; }
        public SupportPart Right { get; set; }

        public SupportPart GetByLocation(ChassisLocations location)
        {
            if (location == ChassisLocations.RightArm)
                return Right;
            if (location == ChassisLocations.LeftArm)
                return Left;

            return null;
        }
    }
}