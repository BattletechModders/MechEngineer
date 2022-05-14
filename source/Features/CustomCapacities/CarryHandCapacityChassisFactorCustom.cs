using CustomComponents;

namespace MechEngineer.Features.CustomCapacities;

[CustomComponent("CarryHandCapacityChassisFactor")]
public class CarryHandCapacityChassisFactorCustom : SimpleCustomComponent, IValueComponent<float>
{
    internal float Value;

    public void LoadValue(float value)
    {
        Value = value;
    }
}
