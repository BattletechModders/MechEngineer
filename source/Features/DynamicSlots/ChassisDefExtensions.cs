using System;
using BattleTech;

namespace MechEngineer.Features.DynamicSlots;

internal static class ChassisDefExtensions
{
    internal static ref readonly LocationDef GetRefLocationDef(this ChassisDef chassisDef, ChassisLocations location)
    {
        var indexed = ToIndexed(location);
        var index = (int)indexed;
        return ref chassisDef.Locations[index];
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