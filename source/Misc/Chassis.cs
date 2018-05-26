using System;
using System.Linq;
using BattleTech;

namespace MechEngineMod
{
    internal static class Chassis
    {
        internal static void ModifyInitialTonnage(ChassisDef chassisDef)
        {
            if (!Control.settings.InitialTonnageOverride)
            {
                return;
            }

            if (Control.settings.InitialTonnageOverrideSkipChassis.Contains(chassisDef.Description.Id))
            {
                return;
            }

            var value = chassisDef.Tonnage * Control.settings.InitialToTotalTonnageFactor;
            var propInfo = typeof(ChassisDef).GetProperty("InitialTonnage");
            var propValue = Convert.ChangeType(value, propInfo.PropertyType);
            propInfo.SetValue(chassisDef, propValue, null);
        }
    }
}