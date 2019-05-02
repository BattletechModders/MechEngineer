using BattleTech;

namespace MechEngineer.Features.LocationalEffects
{
    internal class VehicleLocationNaming : LocationNaming
    {
        private readonly VehicleChassisLocations location;

        internal VehicleLocationNaming(VehicleChassisLocations location)
        {
            this.location = location;
        }

        internal override string InterpolateEffectId(string id)
        {
            return id.Replace(LocationPlaceholder, LocationId);
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
                switch (location)
                {
                    case VehicleChassisLocations.Left:
                        return "LeftSide";
                    case VehicleChassisLocations.Right:
                        return "RightSide";
                    default:
                        return location.ToString();
                }
            }
        }

        internal override string LocationName => location.ToString().ToLowerInvariant();
    }
}