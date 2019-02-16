
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("ArmActuator")]
    public class ArmActuator : SimpleCustomComponent
    {
        public float? AccuracyBonus;
        public ArmActuatorSlot Type =  ArmActuatorSlot.Hand;


        public override string ToString()
        {
            return $"ArmActuator: {Type}+{AccuracyBonus ?? 0f}";
        }
    }
}