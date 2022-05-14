using CustomComponents;

namespace MechEngineer.Features.CustomCapacities;

[CustomComponent("CarryMechUsage")]
public class CarryMechUsageCustom : SimpleCustomComponent, IValueComponent<float>
{
    internal float Value;

    public void LoadValue(float value)
    {
        Value = value;
    }
}
