using CustomComponents;

namespace MechEngineer.Features.CustomCapacities
{
    [CustomComponent("CarryCapacityFactor")]
    public class CarryCapacityFactorCustom : SimpleCustomComponent, IValueComponent<float>
    {
        internal float Value;
        public void LoadValue(float value)
        {
            Value = value;
        }
    }
}
