using CustomComponents;

namespace MechEngineer.Features.CustomCapacities;

[CustomComponent("CarryMechCapacityChassisFactor")]
public class CarryMechCapacityChassisFactorCustom : SimpleCustomComponent, IValueComponent<float>
{
    internal float Value;

    public void LoadValue(float value)
    {
        Value = value;
    }
}
