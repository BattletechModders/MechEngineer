using CustomComponents;

namespace MechEngineer.Features.CustomCapacities
{
    [CustomComponent("CarryCapacityOnArmChassisFactor")]
    public class CarryCapacityOnArmChassisFactorCustom : SimpleCustomComponent, IValueComponent<float>
    {
        internal float Value;
        public void LoadValue(float value)
        {
            Value = value;
        }
    }
}
