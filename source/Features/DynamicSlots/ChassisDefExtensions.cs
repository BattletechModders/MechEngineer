using System;
using BattleTech;

namespace MechEngineer.Features.DynamicSlots;

internal static class ChassisDefExtensions
{
    internal static ref readonly LocationDef GetRefLocationDef(this ChassisDef chassisDef, ChassisLocations location)
    {
        // if properly sorted, we can access the locationDef directly
        var indexed = ToIndexed(location);
        var index = (int)indexed;
        if (index < chassisDef.Locations.Length)
        {
            ref readonly var locationDef = ref chassisDef.Locations[index];
            if (locationDef.Location == location)
            {
                return ref locationDef;
            }
        }

        // if not sorted properly, we have to search
        for (var i = 0; i < chassisDef.Locations.Length; i++)
        {
            ref readonly var locationDef = ref chassisDef.Locations[i];
            if (locationDef.Location == location)
            {
                return ref locationDef;
            }
        }

        // some fallback that is compatible with "ref" no copy syntax
        return ref chassisDef.Locations[0];
    }

    private static ChassisLocationsIndexed ToIndexed(ChassisLocations location)
    {
        return location switch
        {
            ChassisLocations.Head => ChassisLocationsIndexed.Head,
            ChassisLocations.LeftArm => ChassisLocationsIndexed.LeftArm,
            ChassisLocations.LeftTorso => ChassisLocationsIndexed.LeftTorso,
            ChassisLocations.CenterTorso => ChassisLocationsIndexed.CenterTorso,
            ChassisLocations.RightTorso => ChassisLocationsIndexed.RightTorso,
            ChassisLocations.RightArm => ChassisLocationsIndexed.RightArm,
            ChassisLocations.LeftLeg => ChassisLocationsIndexed.LeftLeg,
            ChassisLocations.RightLeg => ChassisLocationsIndexed.RightLeg,
            _ => throw new ArgumentOutOfRangeException(nameof(location))
        };
    }
    private enum ChassisLocationsIndexed
    {
        Head,
        LeftArm,
        LeftTorso,
        CenterTorso,
        RightTorso,
        RightArm,
        LeftLeg,
        RightLeg,
    }
}