using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.MechLabSlots;

[CustomComponent("ChassisLocationNaming")]
public class ChassisLocationNaming : SimpleCustomChassis
{
    public LocationName[] Names = new LocationName[0];

    public class LocationName
    {
        public ChassisLocations Location;
        public string Label = null!;
        public string ShortLabel = null!;
    }
}