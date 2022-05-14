using CustomComponents;

namespace MechEngineer.Features.CustomCapacities;

[CustomComponent("LiftUsage")]
public class LiftUsageCustom : SimpleCustomComponent, IValueComponent<float>
{
    internal float Value = 0;

    public void LoadValue(float value)
    {
        Value = value;
    }
}
