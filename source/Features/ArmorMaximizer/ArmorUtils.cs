using System.Linq;
using BattleTech;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.ArmorMaximizer;

public static class ArmorUtils
{
    //Takes TONNAGE_PER_ARMOR_POINT and multiplies it by the ArmorFactor provided by equipped items.
    public static float TonPerPoint(this MechDef mechDef)
    {
        float tonPerPoint = UnityGameInstance.BattleTechGame.MechStatisticsConstants.TONNAGE_PER_ARMOR_POINT;
        float armorFactor = WeightsUtils.CalculateArmorFactor(mechDef);
        float adjustedTonPerPoint = tonPerPoint * armorFactor;
        return adjustedTonPerPoint;
    }
    //The weight of the armor that is currently equipped on the Mech.
    public static float CalcArmorWeight(this MechDef mechDef)
    {
        float armorPoints = mechDef.MechDefAssignedArmor;
        float tonPerPoint = mechDef.TonPerPoint();
        float armorWeight = armorPoints * tonPerPoint;
        return armorWeight;
    }
    //Armor weight + free tonnage.  This is the amount of possible armor in tons that can be assigned.
    public static float UsableWeight(this MechDef mechDef)
    {
        float weight = CalcArmorWeight(mechDef);
        weight += Weights.CalculateFreeTonnage(mechDef);
        if (weight <= 0)
        {
            return 0;
        }
        return weight;
    }
    //Max total armor points that the Mech can have based on CBT rules as per MechEngineer.
    public static float MaxArmorPoints(this MechDef mechDef)
    {
        return MechDefBuilder.Locations
            .Select(location => mechDef.Chassis.GetLocationDef(location))
            .Select(ArmorStructureRatioFeature.GetArmorPoints)
            .Sum();
    }
    //Calculates available armor points based usable weight.
    public static float AvailableAP(this MechDef mechDef)
    {
        float mathFactor = 1000f;
        float maxAP = mechDef.MaxArmorPoints();
        float tonsPerPoint = mechDef.TonPerPoint() * mathFactor;
        float availableAP = mechDef.UsableWeight() * mathFactor;
        bool divisible = (IsDivisible(availableAP, tonsPerPoint));
        availableAP /= tonsPerPoint;
        if (!divisible)
        {
            Control.Logger.Error.Log("Not Divisible!!!!");
            availableAP = RoundDown(availableAP,0.001f);
            Control.Logger.Error.Log("availableAP: " + availableAP);
            return availableAP;
        }
        if (availableAP > mechDef.MaxArmorPoints())
        {
            return maxAP;
        }
        return availableAP;
    }
    //Percentage equal to available armor points divided by max armor points.
    public static float ArmorMultiplier(this MechDef mechDef)
    {
        float headPoints = mechDef.Head.AssignedArmor;
        float availablePoints = mechDef.AvailableAP();
        float maxArmor = mechDef.MaxArmorPoints();
        if(ArmorMaximizerFeature.Shared.Settings.HeadPointsUnChanged)
        {
            maxArmor -= headPoints;
            availablePoints -= headPoints;
        }
        float multiplier = availablePoints / maxArmor;
        return multiplier;
    }
    //Max AP by location
    public static float CalcMaxAPbyLocation(this MechDef mechDef, LocationLoadoutDef location, LocationDef locationDef)
    {
        float maxAP = locationDef.InternalStructure * 2;
        if (location == mechDef.Head)
        {
            maxAP = RoundDown(locationDef.InternalStructure * 3,5);
        }
        return maxAP;
    }
    public static float AssignAPbyLocation(this MechDef mechDef, LocationLoadoutDef location, LocationDef locationDef)
    {
        float maxAP = locationDef.InternalStructure * 2;
        float availableAP = mechDef.CalcMaxAPbyLocation(location, locationDef);
        availableAP *= mechDef.ArmorMultiplier();
        availableAP = Mathf.Floor(availableAP);

        if (location == mechDef.Head)
        {
            maxAP = locationDef.MaxArmor;
            if (ArmorMaximizerFeature.Shared.Settings.HeadPointsUnChanged)
            {
                availableAP = location.AssignedArmor;
                if(availableAP > maxAP)
                {
                    availableAP = maxAP;
                }
            }
        }
        if(availableAP > maxAP)
        {
            availableAP = maxAP;
        }
        return availableAP;
    }

    public static float CurrentArmorPoints(this MechDef mechDef)
    {
        float currentArmorPoints = mechDef.MechDefAssignedArmor;
        return currentArmorPoints;

    }
    public static bool CanMaxArmor(this MechDef mechDef)
    {
        float buffer = 15.0f;
        float adjustedTPP = mechDef.TonPerPoint();
        float headArmor = mechDef.Head.AssignedArmor;
        float minFree = (headArmor + buffer) * adjustedTPP;
        if (UsableWeight(mechDef) < minFree)
        {
            return false;
        }
        return true;
    }
    public static bool IsDivisible(float x, float y)
    {
        if (y < 0) y *= -1f;
        return (x % y) == 0f;
    }
    public static float RoundUp(float x, float y)
    {
        if (y < 0) y *= -1f;
        x = Mathf.Ceil(x / y);
        return x * y;
    }
    public static float RoundDown(float x, float y)
    {
        if (y < 0) y *= -1f;
        x = Mathf.Floor(x / y);
        return x * y;
    }
}