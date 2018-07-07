using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    internal static class ChassisHandler
    {
        private static readonly Dictionary<string, float> OriginalInitialTonnages = new Dictionary<string, float>();

        internal static void OverrideChassisSettings(ChassisDef chassisDef)
        {
            if (Control.settings.AutoFixChassisDefSkip.Contains(chassisDef.Description.Id))
            {
                return;
            }

            AutoFixChassisDef(chassisDef);
            AutoFixSlots(chassisDef);
        }

        public static float? GetOriginalTonnage(ChassisDef chassisDef)
        {
            if (OriginalInitialTonnages.TryGetValue(chassisDef.Description.Id, out var value))
            {
                return value;
            }

            return null;
        }

        private static void SetOriginalTonnage(this ChassisDef chassisDef)
        {
            if (OriginalInitialTonnages.ContainsKey(chassisDef.Description.Id))
            {
                return;
            }

            OriginalInitialTonnages[chassisDef.Description.Id] = chassisDef.InitialTonnage;
        }

        private static void AutoFixChassisDef(ChassisDef chassisDef)
        {
            if (!Control.settings.AutoFixChassisDefInitialTonnage)
            {
                return;
            }

            {
                SetOriginalTonnage(chassisDef);
                var tonnage = chassisDef.Tonnage * Control.settings.AutoFixChassisDefInitialToTotalTonnageFactor;
                var info = typeof(ChassisDef).GetProperty("InitialTonnage");
                var value = Convert.ChangeType(tonnage, info.PropertyType);
                info.SetValue(chassisDef, value, null);
            }

            {
                // TODO: make engine calculation independent of def/refs again, to allow reuse of calc here
                var maxCount = Math.Min(8, Math.Ceiling(400 / chassisDef.Tonnage));
                var info = typeof(ChassisDef).GetProperty("MaxJumpjets");
                var value = Convert.ChangeType(maxCount, info.PropertyType);
                info.SetValue(chassisDef, value, null);
            }
        }

        private static void AutoFixSlots(ChassisDef chassisDef)
        {
            if (!Control.settings.AutoFixChassisDefSlots)
            {
                return;
            }

            var traverse = Traverse.Create(chassisDef);
            var locations = traverse.Field("Locations").GetValue<LocationDef[]>();
            for (var i = 0; i < locations.Length; i++)
            {
                ModifyInventorySlots(ref locations[i], ChassisLocations.LeftTorso, 10, 12);
                ModifyInventorySlots(ref locations[i], ChassisLocations.RightTorso, 10, 12);
                ModifyInventorySlots(ref locations[i], ChassisLocations.LeftLeg, 4, 2);
                ModifyInventorySlots(ref locations[i], ChassisLocations.RightLeg, 4, 2);
                ModifyInventorySlots(ref locations[i], ChassisLocations.Head, 1, 2);
                ModifyInventorySlots(ref locations[i], ChassisLocations.CenterTorso, 4, 12);
            }

            traverse.Method("refreshLocationReferences").GetValue();

            //Control.mod.Logger.LogDebug("AutoFixSlots InventorySlots=" + chassisDef.LeftTorso.InventorySlots);
        }

        private static void ModifyInventorySlots(ref LocationDef locationDef, ChassisLocations location, int currentSlots, int newSlots)
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