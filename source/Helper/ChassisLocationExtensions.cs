using BattleTech;

namespace MechEngineer.Helper;

internal static class ChassisLocationExtensions
{
    internal static string GetShortString(this ChassisLocations location)
    {
        return Mech.GetAbbreviatedChassisLocation(location).ToString();
    }
}