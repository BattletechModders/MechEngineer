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

    internal override string LocationId
    {
        get
        {
            switch (Location)
            {
                case VehicleChassisLocations.Left:
                    return "LeftSide";
                case VehicleChassisLocations.Right:
                    return "RightSide";
                default:
                    return Location.ToString();
            }
        }
    }

    private string LocationName => Location.ToString().ToLowerInvariant();
}