using CustomComponents;

namespace MechEngineer.Features.CustomCapacities
{
    [CustomComponent("CarryCapacityAddend")]
    public class CarryCapacityAddendCustom : SimpleCustomComponent, IValueComponent<float>
    {
        internal float Value;
        public void LoadValue(float value)
        {
            Value = value;
        }
    }
}
