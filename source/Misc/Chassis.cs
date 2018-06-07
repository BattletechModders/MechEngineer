using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    internal static class Chassis
    {
        internal static void OverrideChassisSettings(ChassisDef chassisDef)
        {
            if (Control.settings.AutoFixChassisDefSkip.Contains(chassisDef.Description.Id))
            {
                return;
            }

            AutoFixInitialTonnage(chassisDef);
            AutoFixSlots(chassisDef);
        }

        internal static void AutoFixInitialTonnage(ChassisDef chassisDef)
        {
            if (!Control.settings.AutoFixChassisDefInitialTonnage)
            {
                return;
            }

            {
                var tonnage = chassisDef.Tonnage * Control.settings.AutoFixInitialToTotalTonnageFactor + Control.settings.AutoFixInitialFixedAddedTonnage;
                var info = typeof(ChassisDef).GetProperty("InitialTonnage");
                var value = Convert.ChangeType(tonnage, info.PropertyType);
                info.SetValue(chassisDef, value, null);
            }

            {
                var info = typeof(ChassisDef).GetProperty("MaxJumpjets");
                var value = Convert.ChangeType(8, info.PropertyType);
                info.SetValue(chassisDef, value, null);
            }
        }

        internal static void AutoFixSlots(ChassisDef chassisDef)
        {
            if (!Control.settings.AutoFixChassisDefSlots)
            {
                return;
            }

            var traverse = Traverse.Create(chassisDef);
            var locations = traverse.Field("Locations").GetValue<LocationDef[]>();
            for (var i=0; i<locations.Length; i++)
            {
                ModifyInventorySlots(ref locations[i], ChassisLocations.LeftTorso, 10, 12);
                ModifyInventorySlots(ref locations[i], ChassisLocations.RightTorso, 10, 12);
                ModifyInventorySlots(ref locations[i], ChassisLocations.LeftLeg, 4, 2);
                ModifyInventorySlots(ref locations[i], ChassisLocations.RightLeg, 4, 2);
            }

            traverse.Method("refreshLocationReferences").GetValue();

            //Control.mod.Logger.LogDebug("AutoFixSlots InventorySlots=" + chassisDef.LeftTorso.InventorySlots);
        }

        internal static void ModifyInventorySlots(ref LocationDef locationDef, ChassisLocations location, int currentSlots, int newSlots)
        {
            if (locationDef.Location != location)
            {
                return;
            }

            if (locationDef.InventorySlots != currentSlots)
            {
                return;
            }

            var info = typeof(LocationDef).GetField("InventorySlots");
            var value = Convert.ChangeType(newSlots, info.FieldType);
            var box = (object) locationDef;
            info.SetValue(box, value);
            locationDef = (LocationDef) box;
            
            //Control.mod.Logger.LogDebug("ModifyInventorySlots InventorySlots=" + locationDef.InventorySlots);
        }
    }
}