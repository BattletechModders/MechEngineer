using CustomComponents;

namespace MechEngineer.Features.CustomCapacities
{
    [CustomComponent("CarryCapacityBaseAddendByChassisFactor")]
    public class CarryCapacityBaseAddendByChassisFactorCustom : SimpleCustomComponent, IValueComponent<float>
    {
        internal float Value;
        public void LoadValue(float value)
        {
            Value = value;
        }
    }
}
