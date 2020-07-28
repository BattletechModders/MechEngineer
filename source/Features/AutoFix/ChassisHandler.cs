﻿using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using ErosionBrushPlugin;
using MechEngineer.Features.Engines;
using MechEngineer.Features.HardpointFix.limits;
using MechEngineer.Features.MechLabSlots;
using UnityEngine;

namespace MechEngineer.Features.AutoFix
{
    internal class ChassisHandler
    {
        private static readonly Dictionary<string, float> OriginalInitialTonnages = new Dictionary<string, float>();

        internal static void OverrideChassisSettings(ChassisDef chassisDef)
        {
            if (chassisDef.ChassisTags.IgnoreAutofix())
            {
                return;
            }

            Control.mod.Logger.Log($"Auto fixing chassisDef={chassisDef.Description.Id}");

            AutoFixChassisDef(chassisDef);
            AutoFixSlots(chassisDef);
            AutoFixLocationNaming(chassisDef);
        }

        public static float? GetOriginalInitialTonnage(ChassisDef chassisDef)
        {
            if (OriginalInitialTonnages.TryGetValue(chassisDef.Description.Id, out var value))
            {
                return value;
            }

            return null;
        }

        private static void SetOriginalInitialTonnage(ChassisDef chassisDef)
        {
            if (OriginalInitialTonnages.ContainsKey(chassisDef.Description.Id))
            {
                return;
            }

            OriginalInitialTonnages[chassisDef.Description.Id] = chassisDef.InitialTonnage;
        }

        private static void AutoFixChassisDef(ChassisDef chassisDef)
        {
            if (AutoFixerFeature.settings.ChassisDefInitialTonnage)
            {
                SetOriginalInitialTonnage(chassisDef);
                var tonnage = chassisDef.Tonnage * AutoFixerFeature.settings.ChassisDefInitialToTotalTonnageFactor;
                var info = typeof(ChassisDef).GetProperty("InitialTonnage");
                var value = Convert.ChangeType(tonnage, info.PropertyType);
                info.SetValue(chassisDef, value, null);
                
                Control.mod.Logger.LogDebug($"set InitialTonnage={tonnage}");
            }

            if (AutoFixerFeature.settings.ChassisDefMaxJumpjets)
            {
                var coreDef = new EngineCoreDef {Rating = AutoFixerFeature.settings.ChassisDefMaxJumpjetsRating };
                var maxCount = Mathf.Min(
                    AutoFixerFeature.settings.ChassisDefMaxJumpjetsCount,
                    coreDef.GetMovement(chassisDef.Tonnage).JumpJetCount
                );
                var info = typeof(ChassisDef).GetProperty("MaxJumpjets");
                var value = Convert.ChangeType(maxCount, info.PropertyType);
                info.SetValue(chassisDef, value, null);
                
                Control.mod.Logger.LogDebug($"set MaxJumpjets={maxCount}");
            }
        }

        private static void AutoFixSlots(ChassisDef chassisDef)
        {
            var changes = AutoFixerFeature.settings.ChassisDefSlotsChanges;
            if (changes == null)
            {
                return;
            }

            var adapter = new ChassisDefAdapter(chassisDef);
            var locations = adapter.Locations;

            for (var i = 0; i < locations.Length; i++)
            {
                var location = locations[i].Location;
                foreach (var change in changes.Where(x => x.Location == location).Select(x => x.Change))
                {
                    ModifyInventorySlots(ref locations[i], location, change);
                }
            }
            
            adapter.refreshLocationReferences();
        }

        private static void AutoFixLocationNaming(ChassisDef chassisDef)
        {
            if (AutoFixerFeature.settings.MechLocationNamingTemplateTags.Any(chassisDef.ChassisTags.Contains))
            {
                if (!chassisDef.Is<ChassisLocationNaming>())
                {
                    var copy = AutoFixerFeature.settings.MechLocationNamingTemplate.ReflectionCopy();
                    chassisDef.AddComponent(copy);
                }
            }
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

            Control.mod.Logger.LogDebug($"set InventorySlots={locationDef.InventorySlots} on location={location}");
        }
    }
}
