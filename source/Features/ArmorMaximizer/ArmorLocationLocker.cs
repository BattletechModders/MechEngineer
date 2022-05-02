using System.Collections.Generic;
using BattleTech;

namespace MechEngineer.Features.ArmorMaximizer;

internal static class ArmorLocationLocker
{
    internal static bool IsLocked(ChassisLocations chassisLocation, bool isRearArmor)
    {
        var armorLocation = GetArmorLocationFromChassisLocation(chassisLocation, isRearArmor);
        return IsLocked(armorLocation);
    }

    internal static void ToggleLock(ChassisLocations chassisLocation, bool isRearArmor)
    {
        var armorLocation = GetArmorLocationFromChassisLocation(chassisLocation, isRearArmor);
        ToggleLock(armorLocation);
    }

    private static readonly HashSet<ArmorLocation> LockedArmorLocations = new HashSet<ArmorLocation>();

    private static bool IsLocked(ArmorLocation location)
    {
        return LockedArmorLocations.Contains(location);
    }

    private static void ToggleLock(ArmorLocation location)
    {
        if (!LockedArmorLocations.Add(location))
        {
            LockedArmorLocations.Remove(location);
        }
    }

    private static ArmorLocation GetArmorLocationFromChassisLocation(ChassisLocations chassisLocation, bool isRearArmor)
    {
        var armorLocation = MechStructureRules.GetArmorFromChassisLocation(chassisLocation);
        const ArmorLocation rearArmor = ArmorLocation.CenterTorsoRear | ArmorLocation.LeftTorsoRear | ArmorLocation.RightTorsoRear;
        return armorLocation & (isRearArmor ? rearArmor : ~rearArmor);
    }
}