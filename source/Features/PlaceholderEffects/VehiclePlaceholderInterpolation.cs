using BattleTech;

namespace MechEngineer.Features.PlaceholderEffects;

internal class VehiclePlaceholderInterpolation : PlaceholderInterpolation
{
    private readonly VehicleChassisLocations Location;

    internal VehiclePlaceholderInterpolation(MechComponent mechComponent)
    {
        MechComponent = mechComponent;
        Location = mechComponent.vehicleComponentRef.MountedLocation;
    }

    internal override string InterpolateEffectId(string id)
    {
        return base.InterpolateEffectId(id).Replace(LocationPlaceholder, LocationId);
    }

    internal override string InterpolateStatisticName(string id)
    {
        return id.Replace(LocationPlaceholder, LocationId);
    }

    internal override string InterpolateText(string text)
    {
        return text.Replace(LocationPlaceholder, LocationName);
    }

    internal override string LocationId => Location switch
    {
        VehicleChassisLocations.Turret => "Turret",
        VehicleChassisLocations.Left => "LeftSide",
        VehicleChassisLocations.Right => "RightSide",
        VehicleChassisLocations.Front => "Front",
        VehicleChassisLocations.Rear => "Rear",
        _ => "LocationId"
    };

    private string LocationName => Location switch
    {
        VehicleChassisLocations.Turret => "turret",
        VehicleChassisLocations.Front => "front",
        VehicleChassisLocations.Left => "left",
        VehicleChassisLocations.Right => "right",
        VehicleChassisLocations.Rear => "rear",
        _ => "LocationName"
    };
}