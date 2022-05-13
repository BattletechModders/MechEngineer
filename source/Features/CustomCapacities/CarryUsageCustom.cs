using CustomComponents;

namespace MechEngineer.Features.CustomCapacities;

[CustomComponent("CarryUsage")]
public class CarryUsageCustom : SimpleCustomComponent, IValueComponent<float>
{
    internal float Value = 0;

    public void LoadValue(float value)
    {
        Value = value;
    }
}