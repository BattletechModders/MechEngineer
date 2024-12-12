using System.Collections.Generic;
using CustomComponents;

namespace MechEngineer.Features.CustomCapacities.Aliases;

[CustomComponent("CarryHandCapacityChassisFactor")]
public class CarryHandCapacityChassisFactorCustom : SimpleCustomComponent, IValueComponent<float>, IOnLoaded
{
    private float Value;

    public void LoadValue(float value)
    {
        Value = value;
    }

    public void OnLoaded()
    {
        Def.AddComponent(new CapacityModCustom
            {
                Collection = CustomCapacitiesFeature.CarryInHandCollectionId,
                IsLocationRestricted = true,
                Quantity = Value,
                QuantityFactorType = QuantityFactorType.ChassisTonnage
            }
        );
    }
}
