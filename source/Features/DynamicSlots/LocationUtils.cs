using System.Linq;
using BattleTech;

namespace MechEngineer.Features.DynamicSlots;

internal static class LocationUtils
{
    internal static readonly ChassisLocations[] Locations =
    {
        ChassisLocations.CenterTorso,
        ChassisLocations.Head,
        ChassisLocations.LeftTorso,
        ChassisLocations.LeftLeg,
        ChassisLocations.RightTorso,
        ChassisLocations.RightLeg,
        ChassisLocations.LeftArm,
        ChassisLocations.RightArm
    };

    internal static ChassisLocations GetInnerAdjacentLocation(ChassisLocations location)
    {
        return location switch
        {
            ChassisLocations.Head => ChassisLocations.CenterTorso,
            ChassisLocations.LeftTorso => ChassisLocations.CenterTorso,
            ChassisLocations.RightTorso => ChassisLocations.CenterTorso,
            ChassisLocations.LeftArm => ChassisLocations.LeftTorso,
            ChassisLocations.LeftLeg => ChassisLocations.LeftTorso,
            ChassisLocations.RightArm => ChassisLocations.RightTorso,
            ChassisLocations.RightLeg => ChassisLocations.RightTorso,
            _ => ChassisLocations.None
        };
    }

    internal static int LocationCount(ChassisLocations container)
    {
        if (container == ChassisLocations.All)
        {
            return Locations.Length;
        }

        return Locations.Count(location => (container & location) != ChassisLocations.None);
    }

    internal static readonly ChassisLocations[] DynamicLocationalOrder =
    {
        ChassisLocations.LeftArm,
        ChassisLocations.RightArm,
        ChassisLocations.Head,
        ChassisLocations.LeftLeg,
        ChassisLocations.RightLeg,
        ChassisLocations.LeftTorso,
        ChassisLocations.RightTorso,
        ChassisLocations.CenterTorso
    };
}