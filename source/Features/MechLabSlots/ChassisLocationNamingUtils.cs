using System.Linq;
using BattleTech;
using CustomComponents;
using Localize;

namespace MechEngineer.Features.MechLabSlots;

internal static class ChassisLocationNamingUtils
{
    internal static Text GetLocationLabel(ChassisDef chassisDef, ChassisLocations location)
    {
        if (chassisDef.Is<ChassisLocationNaming>(out var naming))
        {
            var text = naming.Names
                .Where(x => x.Location == location)
                .Select(x => x.Label)
                .FirstOrDefault();

            if (text != null)
            {
                return new Text(text);
            }
        }

        return Mech.GetLongChassisLocation(location);
    }
}