
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("ArmActuator")]
    public class ArmActuator : SimpleCustomComponent
    {
        public float? AccuracyBonus;
        public TypeDef Type = TypeDef.Hand;

        public enum TypeDef
        {
            Hand, // Hand = default
            Lower,
            Upper,
        }
    }
}