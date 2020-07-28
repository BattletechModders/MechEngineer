﻿using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.MechLabSlots
{
    [CustomComponent("ChassisLocationNaming")]
    public class ChassisLocationNaming: SimpleCustomChassis
    {
        public LocationName[] Names = new LocationName[0];

        public class LocationName
        {
            public ChassisLocations location;
            public string text;
        }
    }
}