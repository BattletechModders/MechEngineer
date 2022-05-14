using CustomComponents;

namespace MechEngineer.Features.CustomCapacities;

[CustomComponent("CarryHandUsage")]
public class CarryHandUsageCustom : SimpleCustomComponent, IValueComponent<float>
{
    internal float Value;

    public void LoadValue(float value)
    {
        Value = value;
    }
}
