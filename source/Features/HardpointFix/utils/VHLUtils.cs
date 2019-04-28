using System;
using BattleTech;

namespace MechEngineer.Features.HardpointFix.utils
{
    internal static class VHLUtils
    {
        internal static ChassisLocations GetLocationByString(string locationString)
        {
            return (ChassisLocations) Enum.Parse(typeof(ChassisLocations), locationString, true);
        }

        internal static string GetStringFromLocation(ChassisLocations location)
        {
            return location.ToString().ToLower();
        }
    }
}