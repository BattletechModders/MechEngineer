using System;
using System.Linq;
using BattleTech;

namespace MechEngineMod
{
    internal static class Chassis
    {
        internal static void OverrideChassisSettings(ChassisDef chassisDef)
        {
            if (!Control.settings.AutoFixChassisDefs)
            {
                return;
            }

            if (Control.settings.AutoFixChassisDefsSkip.Contains(chassisDef.Description.Id))
            {
                return;
            }

            {
                var value = chassisDef.Tonnage * Control.settings.AutoFixInitialToTotalTonnageFactor + Control.settings.AutoFixInitialFixedAddedTonnage;
                var propInfo = typeof(ChassisDef).GetProperty("InitialTonnage");
                var propValue = Convert.ChangeType(value, propInfo.PropertyType);
                propInfo.SetValue(chassisDef, propValue, null);
            }

            {
                var propInfo = typeof(ChassisDef).GetProperty("MaxJumpjets");
                var propValue = Convert.ChangeType(8, propInfo.PropertyType);
                propInfo.SetValue(chassisDef, propValue, null);
            }
        }
    }
}