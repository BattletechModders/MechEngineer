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

    [Serializable]
    public class SupportPart
    {
        public ArmActuatorSlot MaxActuator { get; set; } = ArmActuatorSlot.Hand;
        public string DefaultShoulder { get; set; }
        public string DefaultUpper { get; set; }
    }

    [CustomComponent("ArmActuatorSupport")]
    public class ArmSupportCBT : ChassisCusomComponent, IAfterLoad
    {
        public SupportPart Left { get; set; }
        public SupportPart Right { get; set; }
        public ArmActuatorSlot LeftLimit = ArmActuatorSlot.Hand;
        public ArmActuatorSlot RightLimit = ArmActuatorSlot.Hand;



        public SupportPart GetByLocation(ChassisLocations location)
        {
            if (location == ChassisLocations.RightArm)
                return Right;
            if (location == ChassisLocations.LeftArm)
                return Left;

            return null;
        }

        public void OnLoaded(Dictionary<string, object> values)
        {
            if (Left == null)
                Left = new SupportPart() { MaxActuator = LeftLimit };

            if (Right == null)
                Right = new SupportPart() { MaxActuator = RightLimit };

        }
    }
}