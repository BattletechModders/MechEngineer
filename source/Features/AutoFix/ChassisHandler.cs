using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using UnityEngine;

namespace MechEngineer
{
    internal class ChassisHandler
    {
        internal static ChassisHandler Shared = new ChassisHandler();

        private static readonly Dictionary<string, float> OriginalInitialTonnages = new Dictionary<string, float>();

        internal static void OverrideChassisSettings(ChassisDef chassisDef)
        {
            if (Control.settings.AutoFixChassisDefSkip.Contains(chassisDef.Description.Id))
            {
                return;
            }

            Control.mod.Logger.LogDebug("Auto fixing chassisDef=" + chassisDef.Description.Id);

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

        private static void SetOriginalTonnage(ChassisDef chassisDef)
        {
            if (OriginalInitialTonnages.ContainsKey(chassisDef.Description.Id))
            {
                return;
            }

            OriginalInitialTonnages[chassisDef.Description.Id] = chassisDef.InitialTonnage;
        }

        private static void AutoFixChassisDef(ChassisDef chassisDef)
        {
            if (Control.settings.AutoFixChassisDefInitialTonnage)
            {
                SetOriginalTonnage(chassisDef);
                var tonnage = chassisDef.Tonnage * Control.settings.AutoFixChassisDefInitialToTotalTonnageFactor;
                var info = typeof(ChassisDef).GetProperty("InitialTonnage");
                var value = Convert.ChangeType(tonnage, info.PropertyType);
                info.SetValue(chassisDef, value, null);
            }

            if (Control.settings.AutoFixChassisDefMaxJumpjets)
            {
                var coreDef = new EngineCoreDef {Rating = Control.settings.AutoFixChassisDefMaxJumpjetsRating };
                var maxCount = Mathf.Min(
                    Control.settings.AutoFixChassisDefMaxJumpjetsCount,
                    coreDef.GetMovement(chassisDef.Tonnage).JumpJetCount
                );
                var info = typeof(ChassisDef).GetProperty("MaxJumpjets");
                var value = Convert.ChangeType(maxCount, info.PropertyType);
                info.SetValue(chassisDef, value, null);
            }
        }

        private static Dictionary<ChassisLocations, ValueChange<int>> lookupDictionary;

        private static void AutoFixSlots(ChassisDef chassisDef)
        {
            var changes = Control.settings.AutoFixChassisDefSlotsChanges;
            if (changes == null)
            {
                return;
            }

            var adapter = new ChassisDefAdapter(chassisDef);
            var locations = adapter.Locations;

            if (lookupDictionary == null)
            {
                lookupDictionary = changes.ToDictionary(x => x.Location, x => x.Change);
            }

            for (var i = 0; i < locations.Length; i++)
            {
                var location = locations[i].Location;
                if (lookupDictionary.TryGetValue(location, out var change))
                {
                    ModifyInventorySlots(ref locations[i], location, change);
                }
            }
            
            adapter.refreshLocationReferences();

            //Control.mod.Logger.LogDebug("AutoFixSlots InventorySlots=" + chassisDef.LeftTorso.InventorySlots);
        }

        private static void ModifyInventorySlots(ref LocationDef locationDef, ChassisLocations location, ValueChange<int> change)
        {
            if (locationDef.Location != location)
            {
                return;
            }

            var newValue = change.Change(locationDef.InventorySlots);
            if (!newValue.HasValue)
            {
                return;
            }

            var info = typeof(LocationDef).GetField("InventorySlots");
            var value = Convert.ChangeType(newValue, info.FieldType);
            var box = (object) locationDef;
            info.SetValue(box, value);
            locationDef = (LocationDef) box;

            //Control.mod.Logger.LogDebug("ModifyInventorySlots InventorySlots=" + locationDef.InventorySlots);
        }
    }
}
