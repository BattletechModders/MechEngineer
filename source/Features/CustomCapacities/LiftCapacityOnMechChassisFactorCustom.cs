using CustomComponents;

namespace MechEngineer.Features.CustomCapacities;

[CustomComponent("LiftCapacityOnMechChassisFactor")]
public class LiftCapacityOnMechChassisFactorCustom : SimpleCustomComponent, IValueComponent<float>
{
    internal float Value;
    public void LoadValue(float value)
    {
        Value = value;
    }
}
