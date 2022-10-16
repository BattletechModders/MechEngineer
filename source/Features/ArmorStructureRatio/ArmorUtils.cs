using System;
using System.Linq;
using BattleTech;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.OverrideTonnage;

namespace MechEngineer.Features.ArmorStructureRatio;

// TODO introduce official shared space to put his
public static class ArmorUtils
{
    //Takes TONNAGE_PER_ARMOR_POINT and multiplies it by the ArmorFactor provided by equipped items.
    public static float TonPerPointWithFactor(MechDef mechDef)
    {
        var tonPerPoint = UnityGameInstance.BattleTechGame.MechStatisticsConstants.TONNAGE_PER_ARMOR_POINT;
        var armorFactor = WeightsUtils.CalculateArmorFactor(mechDef);
        var adjustedTonPerPoint = tonPerPoint * armorFactor;
        return adjustedTonPerPoint;
    }

    internal static int GetMaximumArmorPoints(MechDef mechDef)
    {
        return MechDefBuilder.Locations
            .Select(location => mechDef.Chassis.GetLocationDef(location))
            .Select(GetMaximumArmorPoints)
            .Sum();
    }

    internal static int GetMaximumArmorPoints(LocationDef locationDef)
    {
        var maxTotalArmor = MaxTotalArmorCalc(locationDef);
        return (int)PrecisionUtils.RoundDown(maxTotalArmor, ArmorPerStep);
    }

    internal static Func<LocationDef, float> MaxTotalArmorCalc = locationDef => locationDef.MaxArmor + locationDef.MaxRearArmor;

    internal static int ArmorPerStep => (int)UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_STEP;
}