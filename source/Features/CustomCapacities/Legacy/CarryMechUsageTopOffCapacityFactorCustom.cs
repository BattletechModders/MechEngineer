﻿using System.Collections.Generic;
using CustomComponents;

namespace MechEngineer.Features.CustomCapacities.Legacy;

[CustomComponent("CarryMechUsageTopOffCapacityFactor")]
public class CarryMechUsageTopOffCapacityFactorCustom : SimpleCustomComponent, IValueComponent<float>, IAfterLoad
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
                IsUsage = true,
                Quantity = Value,
                QuantityFactorType = QuantityFactorType.CarrySharedTopOffCapacity,
            }
        );
    }
}
