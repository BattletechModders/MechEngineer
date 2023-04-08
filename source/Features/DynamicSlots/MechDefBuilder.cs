using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using CustomComponents;
using MechEngineer.Helper;
using UnityEngine;

namespace MechEngineer.Features.DynamicSlots;

internal class MechDefBuilder
{
    private static readonly DataManager DataManager = UnityGameInstance.BattleTechGame.DataManager;

    internal readonly List<MechComponentRef> Inventory;
    private readonly ChassisDef Chassis;

    internal MechDefBuilder(MechDef mechDef) : this(mechDef.Chassis, mechDef.Inventory)
    {
    }

    internal MechDefBuilder(ChassisDef chassisDef, MechComponentRef[] inventory)
    {
        Chassis = chassisDef;
        Inventory = inventory.ToList();

        CalculateStats();

        Log.Main.Trace?.Log(ToString());
    }

    // 1. slots and inventory pass
    private int TotalMax;
    private int TotalInventoryUsage;

    // 2. dynamics pass
    private int TotalDynamicGlobalUsage;
    private int TotalDynamicLocationalUsage;

    // set by RefreshSummary after 2. pass
    private int TotalUsage;
    private int TotalFree;
    private int TotalMissing;

    private void RefreshSummary()
    {
        TotalUsage = TotalInventoryUsage + TotalDynamicGlobalUsage + TotalDynamicLocationalUsage;
        TotalFree = Mathf.Max(TotalMax - TotalUsage, 0);
        TotalMissing = Mathf.Max(TotalUsage - TotalMax, 0);
    }

    internal LocationUsages LocationUsages;

    public sealed override string ToString()
    {
        return $"ChassisId={Chassis.Description.Id}" +
               $" {nameof(TotalMax)}={TotalMax}" +
               $" {nameof(TotalInventoryUsage)}={TotalInventoryUsage}" +
               $" {nameof(TotalDynamicGlobalUsage)}={TotalDynamicGlobalUsage}" +
               $" {nameof(TotalDynamicLocationalUsage)}={TotalDynamicLocationalUsage}";
    }

    private void CalculateStats()
    {
        TotalMax = 0;
        TotalInventoryUsage = 0;
        TotalDynamicGlobalUsage = 0;
        TotalDynamicLocationalUsage = 0;

        LocationUsages = new();

        // 1. slots and inventory pass
        for (var index = 0; index < LocationUsages.Length; index++)
        {
            ref var locationUsage = ref LocationUsages[index];
            var location = locationUsage.Location;
            ref readonly var locationDef = ref Chassis.GetRefLocationDef(location);
            var inventorySlots = locationDef.InventorySlots;
            locationUsage.Max = inventorySlots;
            TotalMax += inventorySlots;

            foreach (var componentRef in Inventory)
            {
                if (componentRef.MountedLocation != location)
                {
                    continue;
                }

                var inventorySize = componentRef.Def.InventorySize;
                locationUsage.Inventory += inventorySize;
                TotalInventoryUsage += inventorySize;
            }

            locationUsage.RefreshSummary();
        }

        // 2. dynamics pass
        foreach (var location in LocationUtils.DynamicLocationalOrder)
        {
            ref var locationUsage = ref LocationUsages[location];

            foreach (var componentRef in Inventory)
            {
                if (componentRef.MountedLocation != location)
                {
                    continue;
                }

                if (componentRef.Def.Is<DynamicSlots>(out var ds))
                {
                    if (ds.InnerAdjacentOnly)
                    {
                        TotalDynamicLocationalUsage += ds.ReservedSlots;
                        var reservedSlots = ds.ReservedSlots;
                        var free = locationUsage.Free;
                        var reservedUsageOnLocation = Math.Min(reservedSlots, free);
                        locationUsage.DynamicLocationalLocal += reservedUsageOnLocation;
                        locationUsage.RefreshSummary();
                        var leftOver = reservedSlots - reservedUsageOnLocation;
                        if (leftOver > 0)
                        {
                            var adjacentIndex = LocationUtils.GetInnerAdjacentLocation(location);
                            if (adjacentIndex == ChassisLocations.None)
                            {
                                // add it somewhere
                                locationUsage.DynamicLocationalLocal += leftOver;
                                locationUsage.RefreshSummary();
                            }
                            else
                            {
                                ref var adjacentLocationUsage = ref LocationUsages[adjacentIndex];
                                // add it somewhere
                                adjacentLocationUsage.DynamicLocationalTransferred += leftOver;
                                adjacentLocationUsage.RefreshSummary();
                            }
                        }
                    }
                    else
                    {
                        TotalDynamicGlobalUsage += ds.ReservedSlots;
                    }
                }
            }
        }

        RefreshSummary();

        // 3. fixed slots pass
        for (var index = LocationUtils.DynamicLocationalOrder.Length - 1; index >= 0; index--)
        {
            var location = LocationUtils.DynamicLocationalOrder[index];
            ref var locationUsage = ref LocationUsages[location];
            var adjacentIndex = LocationUtils.GetInnerAdjacentLocation(locationUsage.Location);
            if (adjacentIndex == ChassisLocations.None)
            {
                locationUsage.SetFixed(0, TotalFree);
            }
            else
            {
                ref var adjacentLocationUsage = ref LocationUsages[adjacentIndex];
                locationUsage.SetFixed(adjacentLocationUsage.FreeIncludingDynamicMovable, TotalFree);
            }
        }

        // trace
        if (Log.Main.Trace != null)
        {
            for (var index = 0; index < LocationUsages.Length; index++)
            {
                ref var locationUsage = ref LocationUsages[index];
                Log.Main.Trace.Log($"ChassisId={Chassis.Description.Id} {locationUsage}");
            }
        }
    }

    internal bool HasOveruseAtAnyLocation(Errors? errors)
    {
        var overuse = 0;
        if (TotalMissing > 0)
        {
            if (errors == null)
            {
                return true;
            }

            overuse += TotalMissing;
        }

        for (var index = 0; index < LocationUsages.Length; index++)
        {
            ref var locationUsage = ref LocationUsages[index];
            if (locationUsage.Missing > 0)
            {
                if (errors == null)
                {
                    return true;
                }

                overuse += locationUsage.Missing;
            }
        }

        if (overuse > 0)
        {
            errors?.Add(MechValidationType.InvalidInventorySlots, $"RESERVED SLOTS: Mech requires {overuse} additional free slots");
        }
        return overuse > 0;
    }

    #region functions for manipulation

    // TODO whats missing for proper locational dynamic slots support
    // track and allow moving of dynamic slots, with a bias towards locational slots near original item.
    // work by tracking and converting "movable", "reserved" and "missing" locational slots per location
    // where each location has to find its reserved slots from the locations nearby (CT more than LT)
    internal MechComponentRef? Add(MechComponentDef def, ChassisLocations location = ChassisLocations.None, bool force = false)
    {
        if (def.Is<DynamicSlots>())
        {
            // TODO add support, doesn't work with arm actuators either
            throw new("adding dynamic slots is not supported");
        }

        // find location
        if (location == ChassisLocations.None || LocationUtils.LocationCount(location) > 1)
        {
            location = GetLocations()
                .Where(l => (l & def.AllowedLocations) != 0)
                .FirstOrDefault(l => LocationUsages[l].FreeIncludingDynamicMovable >= def.InventorySize);

            if (location == ChassisLocations.None)
            {
                return null;
            }
        }

        var locationInfo = LocationUsages[location];
        var overUseAtLocation = locationInfo.FreeIncludingDynamicMovable < def.InventorySize; // considers locational dynamic slots
        var overUseOverall = TotalFree < def.InventorySize; // considers global dynamic slots
        if (!force && (overUseAtLocation || overUseOverall))
        {
            return null;
        }

        Log.Main.Trace?.Log($"  added id={def.Description.Id} location={location} InventorySize={def.InventorySize} InventoryUsage={locationInfo.Inventory} TotalInventoryUsage={TotalInventoryUsage} overUseAtLocation={overUseAtLocation} overUseOverall={overUseOverall}");

        var componentRef = new MechComponentRef(def.Description.Id, null, def.ComponentType, location);
        componentRef.DataManager = DataManager;
        componentRef.RefreshComponentDef();
        Inventory.Add(componentRef);

        Log.Main.Trace?.Log($"  adding id={def.Description.Id} location={location} InventorySize={def.InventorySize} InventoryUsage={locationInfo.Inventory} TotalInventoryUsage={TotalInventoryUsage} overUseAtLocation={overUseAtLocation} overUseOverall={overUseOverall}");
        CalculateStats();
        Log.Main.Trace?.Log($"  added id={def.Description.Id} location={location} InventorySize={def.InventorySize} InventoryUsage={locationInfo.Inventory} TotalInventoryUsage={TotalInventoryUsage} overUseAtLocation={overUseAtLocation} overUseOverall={overUseOverall}");

        return componentRef;
    }

    internal void Remove(MechComponentRef componentRef)
    {
        if (componentRef.Is<DynamicSlots>())
        {
            // TODO add support, doesn't work with arm actuators either
            throw new("removing dynamic slots is not supported");
        }

        var def = componentRef.Def;
        var locationInfo = LocationUsages[componentRef.MountedLocation];
        Inventory.Remove(componentRef);

        Log.Main.Trace?.Log($"  removing id={def.Description.Id} location={componentRef.MountedLocation} InventorySize={def.InventorySize} InventoryUsage={locationInfo.Inventory} TotalInventoryUsage={TotalInventoryUsage}");
        CalculateStats();
        Log.Main.Trace?.Log($"  removed id={def.Description.Id} location={componentRef.MountedLocation} InventorySize={def.InventorySize} InventoryUsage={locationInfo.Inventory} TotalInventoryUsage={TotalInventoryUsage}");
    }

    private IEnumerable<ChassisLocations> GetLocations()
    {
        yield return ChassisLocations.CenterTorso;

        if (LocationUsages[ChassisLocations.LeftTorso].FreeIncludingDynamicMovable >= LocationUsages[ChassisLocations.RightTorso].FreeIncludingDynamicMovable)
        {
            yield return ChassisLocations.LeftTorso;
            yield return ChassisLocations.RightTorso;
        }
        else
        {
            yield return ChassisLocations.RightTorso;
            yield return ChassisLocations.LeftTorso;
        }

        if (LocationUsages[ChassisLocations.LeftLeg].FreeIncludingDynamicMovable >= LocationUsages[ChassisLocations.RightLeg].FreeIncludingDynamicMovable)
        {
            yield return ChassisLocations.LeftLeg;
            yield return ChassisLocations.RightLeg;
        }
        else
        {
            yield return ChassisLocations.RightLeg;
            yield return ChassisLocations.LeftLeg;
        }

        yield return ChassisLocations.Head;

        if (LocationUsages[ChassisLocations.LeftArm].FreeIncludingDynamicMovable >= LocationUsages[ChassisLocations.RightArm].FreeIncludingDynamicMovable)
        {
            yield return ChassisLocations.LeftArm;
            yield return ChassisLocations.RightArm;
        }
        else
        {
            yield return ChassisLocations.RightArm;
            yield return ChassisLocations.LeftArm;
        }
    }

    #endregion
}