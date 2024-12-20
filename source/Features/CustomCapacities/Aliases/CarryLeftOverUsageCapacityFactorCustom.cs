﻿using System.Collections.Generic;
using CustomComponents;

namespace MechEngineer.Features.CustomCapacities.Aliases;

[CustomComponent("CarryLeftOverUsageCapacityFactor")]
public class CarryLeftOverUsageCapacityFactorCustom : SimpleCustomComponent, IValueComponent<float>, IOnLoaded
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
                Collection = CustomCapacitiesFeature.CarryLeftOverCollectionId,
                IsUsage = true,
                Quantity = Value,
                QuantityFactorType = QuantityFactorType.Capacity,
            }
        );
    }
}
