using System;
using System.Linq;
using BattleTech;

namespace MechEngineer.Features.ArmorMaximizer;

internal static class LocationExtensions
{
    internal static ArmorLocation ToArmorLocation(this ChassisLocations chassisLocation, bool isRearArmor)
    {
        var armorLocation = MechStructureRules.GetArmorFromChassisLocation(chassisLocation);
        return armorLocation & (isRearArmor ? ArmorLocationRear : ~ArmorLocationRear);
    }

    internal static ChassisLocations ToChassisLocation(this ArmorLocation armorLocation)
    {
        return MechStructureRules.GetChassisLocationFromArmorLocation(armorLocation);
    }

    internal static bool IsRear(this ArmorLocation armorLocation)
    {
        var isRearArmor = (armorLocation & ArmorLocationRear) != ArmorLocation.None;
        return isRearArmor;
    }

    private const ArmorLocation ArmorLocationRear = ArmorLocation.CenterTorsoRear | ArmorLocation.LeftTorsoRear | ArmorLocation.RightTorsoRear;

    internal static readonly ArmorLocation[] ArmorLocationList = ((ArmorLocation[])Enum.GetValues(typeof(ArmorLocation)))
        .Where(x => x is > ArmorLocation.None and < ArmorLocation.Invalid)
        .ToArray();

    internal static readonly ChassisLocations[] ChassisLocationList =
    {
        ChassisLocations.Head,
        ChassisLocations.CenterTorso,
        ChassisLocations.LeftTorso,
        ChassisLocations.RightTorso,
        ChassisLocations.LeftLeg,
        ChassisLocations.RightLeg,
        ChassisLocations.LeftArm,
        ChassisLocations.RightArm
    };
}