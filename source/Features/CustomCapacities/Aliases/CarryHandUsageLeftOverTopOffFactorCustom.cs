﻿using System.Collections.Generic;
using CustomComponents;

namespace MechEngineer.Features.CustomCapacities.Aliases;

[CustomComponent("CarryHandUsageLeftOverTopOffFactor")]
public class CarryHandUsageLeftOverTopOffFactorCustom : SimpleCustomComponent, IValueComponent<float>, IOnLoaded
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
                IsUsage = true,
                Quantity = Value,
                QuantityFactorType = QuantityFactorType.CarryLeftOverTopOff,
            }
        );
    }
}
