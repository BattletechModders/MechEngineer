using System.Collections.Generic;
using CustomComponents;

namespace MechEngineer.Features.CustomCapacities.Aliases;

[CustomComponent("HeatSinkEngineAdditionalCapacity")]
public class HeatSinkEngineAdditionalCapacityCustom : SimpleCustomComponent, IValueComponent<float>, IOnLoaded
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
                Collection = CustomCapacitiesFeature.HeatSinkEngineAdditionalCollectionId,
                Quantity = Value
            }
        );
    }
}
