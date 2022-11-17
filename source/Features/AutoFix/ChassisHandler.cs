using System;
using System.Linq;
using BattleTech;
using CustomComponents;
using ErosionBrushPlugin;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.Engines;
using MechEngineer.Features.MechLabSlots;
using MechEngineer.Helper;
using UnityEngine;

namespace MechEngineer.Features.AutoFix;

internal static class ChassisHandler
{
    internal static void OverrideChassisSettings(ChassisDef chassisDef)
    {
        if (chassisDef.ChassisTags.IgnoreAutofix())
        {
            return;
        }

        Log.Main.Info?.Log($"Auto fixing chassisDef={chassisDef.Description.Id}");

        AutoFixChassisDef(chassisDef);
        AutoFixSlots(chassisDef);
        AutoFixLocationNaming(chassisDef);
    }

    private static void AutoFixChassisDef(ChassisDef chassisDef)
    {
        if (AutoFixerFeature.settings.ChassisDefInitialTonnage)
        {
            var tonnage = chassisDef.Tonnage * AutoFixerFeature.settings.ChassisDefInitialToTotalTonnageFactor;
            var info = typeof(ChassisDef).GetProperty("InitialTonnage");
            var value = Convert.ChangeType(tonnage, info.PropertyType);
            info.SetValue(chassisDef, value, null);

            Log.Main.Debug?.Log($"set InitialTonnage={tonnage}");
        }

        if (AutoFixerFeature.settings.ChassisDefMaxJumpjets)
        {
            var coreDef = new EngineCoreDef {Rating = AutoFixerFeature.settings.ChassisDefMaxJumpjetsRating};
            var maxCount = Mathf.Min(
                AutoFixerFeature.settings.ChassisDefMaxJumpjetsCount,
                coreDef.GetMovement(chassisDef.Tonnage).JumpJetCount
            );
            var info = typeof(ChassisDef).GetProperty("MaxJumpjets");
            var value = Convert.ChangeType(maxCount, info.PropertyType);
            info.SetValue(chassisDef, value, null);

            Log.Main.Debug?.Log($"set MaxJumpjets={maxCount}");
        }

        if (AutoFixerFeature.settings.ChassisDefArmorStructureRatio)
        {
            ArmorStructureRatioFeature.Shared.AutoFixChassisDef(chassisDef);
        }
    }

    private static void AutoFixSlots(ChassisDef chassisDef)
    {
        var changes = AutoFixerFeature.settings.ChassisDefSlotsChanges;
        if (changes == null)
        {
            return;
        }

        var locations = chassisDef.Locations;

        for (var i = 0; i < locations.Length; i++)
        {
            var location = locations[i].Location;
            foreach (var change in changes.Where(x => x.Location == location).Select(x => x.Change))
            {
                ModifyInventorySlots(ref locations[i], location, change);
            }
        }

        chassisDef.refreshLocationReferences();
    }

    private static void AutoFixLocationNaming(ChassisDef chassisDef)
    {
        if (chassisDef.Is<ChassisLocationNaming>())
        {
            return;
        }

        var template = AutoFixerFeature.settings.MechLocationNamingTemplates
            .Where(x => x.Tags.Any(chassisDef.ChassisTags.Contains))
            .Select(x => x.Template)
            .FirstOrDefault();

        if (template != null)
        {
            var copy = template.ReflectionCopy();
            chassisDef.AddComponent(copy);
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

        if (location == ChassisLocations.CenterTorso)
        {
            newValue += MechLabSlotsFeature.settings.TopLeftWidget.Slots +
                        MechLabSlotsFeature.settings.TopRightWidget.Slots;
        }

        var info = typeof(LocationDef).GetField("InventorySlots");
        var value = Convert.ChangeType(newValue, info.FieldType);
        var box = (object)locationDef;
        info.SetValue(box, value);
        locationDef = (LocationDef)box;

        Log.Main.Debug?.Log($"set InventorySlots={locationDef.InventorySlots} on location={location}");
    }
}