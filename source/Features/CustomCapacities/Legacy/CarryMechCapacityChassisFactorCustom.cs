using System.Collections.Generic;
using CustomComponents;

namespace MechEngineer.Features.CustomCapacities.Legacy;

[CustomComponent("CarryMechCapacityChassisFactor")]
public class CarryMechCapacityChassisFactorCustom : SimpleCustomComponent, IValueComponent<float>, IAfterLoad
{
    private float Value;

    public void LoadValue(float value)
    {
        Value = value;
    }

    public void OnLoaded(Dictionary<string, object> values)
    {
        Def.AddComponent(new CapacityModCustom
            {
                Collection = CustomCapacitiesFeature.CarryOnMechCollectionId,
                Quantity = Value,
                QuantityFactorType = QuantityFactorType.ChassisTonnage
            }
        );
    }
}
