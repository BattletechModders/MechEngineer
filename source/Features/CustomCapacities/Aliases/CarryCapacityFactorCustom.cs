using System.Collections.Generic;
using CustomComponents;

namespace MechEngineer.Features.CustomCapacities.Aliases;

[CustomComponent("CarryCapacityFactor")]
public class CarryCapacityFactorCustom : SimpleCustomComponent, IValueComponent<float>, IAfterLoad
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
                Collection = CustomCapacitiesFeature.CarryInHandCollectionId,
                Operation = OperationType.Multiply,
                Quantity = Value
            }
        );
        Def.AddComponent(new CapacityModCustom
            {
                Collection = CustomCapacitiesFeature.CarryLeftOverTopOffCollectionId,
                Operation = OperationType.Multiply,
                Quantity = Value
            }
        );
    }
}
