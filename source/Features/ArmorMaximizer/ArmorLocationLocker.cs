using System.Collections.Generic;
using BattleTech;

namespace MechEngineer.Features.ArmorMaximizer;

internal static class ArmorLocationLocker
{
    internal static bool IsLocked(ChassisLocations chassisLocation, bool isRearArmor)
    {
        var armorLocation = chassisLocation.ToArmorLocation(isRearArmor);
        return IsLocked(armorLocation);
    }

    internal static void ToggleLock(ChassisLocations chassisLocation, bool isRearArmor)
    {
        var armorLocation = chassisLocation.ToArmorLocation(isRearArmor);
        ToggleLock(armorLocation);
    }

    private static readonly HashSet<ArmorLocation> LockedArmorLocations = new();

    internal static bool IsLocked(ArmorLocation location)
    {
        return LockedArmorLocations.Contains(location);
    }

    internal static void ToggleLock(ArmorLocation location)
    {
        if (!LockedArmorLocations.Add(location))
        {
            LockedArmorLocations.Remove(location);
        }
    }
}