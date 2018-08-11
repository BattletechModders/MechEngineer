
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("ArmActuatorSupport")]
    public class ArmActuatorSupport : SimpleCustom<ChassisDef>
    {
        public ArmActuator.TypeDef LeftLimit = ArmActuator.TypeDef.Hand;
        public ArmActuator.TypeDef RightLimit = ArmActuator.TypeDef.Hand;
    }
}