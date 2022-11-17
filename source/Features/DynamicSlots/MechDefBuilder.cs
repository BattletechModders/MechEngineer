using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using CustomComponents;
using UnityEngine;

namespace MechEngineer.Features.DynamicSlots;

internal class MechDefBuilder
{
    internal readonly List<MechComponentRef> Inventory;

    private readonly ChassisDef Chassis;
    private readonly DataManager DataManager;

    internal MechDefBuilder(MechDef mechDef) : this(mechDef.Chassis, mechDef.Inventory.ToList())
    {
    }

    internal MechDefBuilder(ChassisDef chassisDef, List<MechComponentRef> inventory)
    {
        Chassis = chassisDef;
        Inventory = inventory;

        DataManager = UnityGameInstance.BattleTechGame.DataManager;
        TotalMax = Locations.Select(chassisDef.GetLocationDef).Sum(d => d.InventorySlots);

        CalculateStats();

        Log.Main.Debug?.Log(ToString());
    }

    #region counting stats and functions

    // precedence order: 1. inventory size, 2. locational restricted dynamic slots, 3. fully movable dynamic slots

    private readonly int TotalMax;

    private int TotalInventoryUsage;
    private int TotalDynamicGlobalUsage;
    private int TotalDynamicLocationalUsage;
    private int TotalDynamicLocationalNoSpace; // missing are added as usage (tracked separately to have quick validation)
    private int TotalUsage => TotalInventoryUsage + TotalDynamicGlobalUsage + TotalDynamicLocationalUsage + TotalDynamicLocationalNoSpace;
    private int TotalFree => TotalMax - TotalUsage;

    internal int TotalMissing => Mathf.Max(TotalUsage - TotalMax, 0) + TotalDynamicLocationalNoSpace;

    public sealed override string ToString()
    {
        return $"ChassisId={Chassis.Description.Id} TotalInventoryUsage={TotalInventoryUsage} TotalDynamicGlobalUsage={TotalDynamicGlobalUsage}" +
               $" TotalDynamicLocationRestrictedUsage={TotalDynamicLocationalUsage} TotalDynamicLocationRestrictedMissing={TotalDynamicLocationalNoSpace}";
    }

    private void CalculateStats()
    {
        TotalInventoryUsage = 0;
        foreach (var group in Inventory.GroupBy(r => r.MountedLocation))
        {
            var location = group.Key;

            var locationInfo = GetLocationInfo(location);
            locationInfo.InventoryUsage = group.Sum(r => r.Def.InventorySize);
            TotalInventoryUsage += locationInfo.InventoryUsage;
        }

        TotalDynamicGlobalUsage = Inventory
            .Select(r => r.Def.GetComponent<DynamicSlots>())
            .Where(s => s != null && !s.InnerAdjacentOnly)
            .Sum(s => s.ReservedSlots);

        TotalDynamicLocationalUsage = 0;
        TotalDynamicLocationalNoSpace = 0;
        foreach (var locationInfo in LocationInfos.Values)
        {
            locationInfo.DLPreferredUsageLocal = 0;
            locationInfo.DLPreferredUsageReservedInOverflow = 0;
            locationInfo.DLNoSpace = 0;
        }

        // "quick" version for validation only
        foreach (var componentRef in Inventory
                     .Where(r => r.Def.Is<DynamicSlots>(out var ds) && ds.InnerAdjacentOnly)
                     .OrderBy(r => Array.IndexOf(RestrictedDynamicSlotsOrder, r.MountedLocation))
                     .ThenBy(r => r.ComponentDefID))
        {
            var slot = componentRef.Def.GetComponent<DynamicSlots>();
            var requiredSlots = slot.ReservedSlots;
            int FindSpaceForDynamicLocation(LocationInfo localLocationInfo)
            {
                var free = localLocationInfo.CalcMaxFree;
                var take = Mathf.Min(free, requiredSlots);
                // apply generic changes to stats
                requiredSlots -= take;
                TotalDynamicLocationalUsage += take;
                return take;
            }

            var locationInfo = GetLocationInfo(componentRef.MountedLocation);
            {
                // find space on location
                var take = FindSpaceForDynamicLocation(locationInfo);
                // apply location specific changes to stats
                locationInfo.DLPreferredUsageLocal += take;
            }

            if (requiredSlots > 0)
            {
                // find space in inner adjacent location (if there is such a location)
                var location = GetInnerAdjacentLocation(componentRef.MountedLocation);
                if (location != ChassisLocations.None)
                {
                    var adjacentLocationInfo = GetLocationInfo(location);
                    var take = FindSpaceForDynamicLocation(adjacentLocationInfo);
                    // apply location specific changes to stats
                    locationInfo.DLPreferredUsageReservedInOverflow += take;
                }
            }

            if (requiredSlots > 0) // if not necessary, since adding 0 would be find too, but this makes it clear
            {
                // note overuse
                locationInfo.DLNoSpace += requiredSlots;
                TotalDynamicLocationalNoSpace += requiredSlots;
            }
        }
    }

    internal class LocationInfo
    {
        internal LocationInfo(MechDefBuilder builder, ChassisLocations location)
        {
            this.builder = builder;
            this.location = location;
            InventoryMax = builder.Chassis.GetLocationDef(location).InventorySlots;
        }
        internal readonly MechDefBuilder builder;
        internal readonly ChassisLocations location;

        internal readonly int InventoryMax;
        internal int InventoryUsage;
        internal int InventoryFree => InventoryMax - InventoryUsage;

        #region dynamic locational stats

        // the maximum amout that is requested, might immediatly overflow to other location (tack not in other location but here as UsageInOverflow)
        //internal int DLUsageRequested;
        // uses up slots in original location
        internal int DLPreferredUsageLocal;
        // uses up slots in the overflow location however its tracked in the location that is overflowing (otherwise its not easy to undo any overflows)
        // when removing components in this location, make sure to reduce overflow location too
        internal int DLPreferredUsageReservedInOverflow;
        // no space left in this and the other location, when removing components have to check if can be reduced!
        internal int DLNoSpace; // missing are added as usage (tracked separately to have quick validation)

        // when removing, 1. reduce no space 2. reduce overflow 3. reduce dl usage local

        #endregion

        #region more intensive calculations

        // avoid in tight loops
        // could be made to cache, but then requires cache invalidation -> invalidate on remove/add right?

        // find all locations that have to overflow to us (and were able to)
        private int CalcOverflowUsage => GetOuterAdjacentLocation(location)
            .Select(builder.GetLocationInfo)
            .Select(x => x.DLPreferredUsageReservedInOverflow)
            .DefaultIfEmpty(0)
            .Sum();

        // preferred: maximize locational dynamic (and omits global dynamic)dwa
        private int CalcMinUsage => InventoryUsage + CalcDLNonMovableUsage;
        internal int CalcMaxFree => InventoryMax - CalcMinUsage;

        internal bool CalcOverUsage => CalcMaxFree < 0;
        internal int CalcMinimumFixedSlotsLocalAndGlobal => Mathf.Max(InventoryUsage + CalcDLNonMovableUsage, InventoryMax - builder.TotalFree);

        private int CalcDLNonMovableUsage => DLPreferredUsageLocal - CalcDLMovableUsage + CalcOverflowUsage + DLNoSpace;

        // movable with current possibilities
        // CT 0 space -> LT, RT, HEAD can't move anything, moveables = 0 there
        // CT 2 space and LT has 2 movable to CT, and LA has 2 DLUsageLocal, then the 2DLUsageLocal should be able to move to LT and LT to CT
        // soo.. use CT's DLUsageLocal as that can never move -> other semantics maybe?
        // LA and LT can use DLusageLocal can move based on VirtualFree on CT
        //
        // recursively go through each location and then go back tracking available space on each location (done via CalcMaxFree)
        // the amount of the preferredUsage that can be moved
        private int CalcDLMovableUsage
        {
            get
            {
                if (DLPreferredUsageLocal == 0)
                {
                    return 0;
                }
                var innerLocation = GetInnerAdjacentLocation(location);
                if (innerLocation == ChassisLocations.None)
                {
                    return 0;
                }

                var maxFree = builder.GetLocationInfo(innerLocation).CalcMaxFree;
                return Mathf.Min(maxFree, DLPreferredUsageLocal);
            }
        }
        //DLUsageLocal + CalcOverflowUsage + DLMissingLocal;

        #endregion
    }

    private readonly Dictionary<ChassisLocations, LocationInfo> LocationInfos = new();
    internal LocationInfo GetLocationInfo(ChassisLocations location)
    {
        if (!LocationInfos.TryGetValue(location, out var usage))
        {
            usage = new LocationInfo(this, location);
            LocationInfos[location] = usage;
        }
        return usage;
    }

    internal bool HasOveruseAtAnyLocation()
    {
        return TotalMissing > 0 || Locations.Any(x => GetLocationInfo(x).CalcOverUsage);
    }

    #endregion

    #region location functions

    internal static readonly ChassisLocations[] Locations =
    {
        ChassisLocations.CenterTorso,
        ChassisLocations.Head,
        ChassisLocations.LeftTorso,
        ChassisLocations.LeftLeg,
        ChassisLocations.RightTorso,
        ChassisLocations.RightLeg,
        ChassisLocations.LeftArm,
        ChassisLocations.RightArm
    };

    private static readonly ChassisLocations[] RestrictedDynamicSlotsOrder =
    {
        ChassisLocations.LeftArm,
        ChassisLocations.RightArm,
        ChassisLocations.Head,
        ChassisLocations.LeftLeg,
        ChassisLocations.RightLeg,
        ChassisLocations.LeftTorso,
        ChassisLocations.RightTorso,
        ChassisLocations.CenterTorso
    };

    internal static ChassisLocations GetInnerAdjacentLocation(ChassisLocations location)
    {
        switch (location)
        {
            case ChassisLocations.Head:
            case ChassisLocations.LeftTorso:
            case ChassisLocations.RightTorso:
                return ChassisLocations.CenterTorso;
            case ChassisLocations.LeftArm:
            case ChassisLocations.LeftLeg:
                return ChassisLocations.LeftTorso;
            case ChassisLocations.RightArm:
            case ChassisLocations.RightLeg:
                return ChassisLocations.RightTorso;
            default:
                return ChassisLocations.None;
        }
    }

    internal static ChassisLocations[] GetOuterAdjacentLocation(ChassisLocations location)
    {
        switch (location)
        {
            case ChassisLocations.Head:
            case ChassisLocations.LeftArm:
            case ChassisLocations.RightArm:
            case ChassisLocations.LeftLeg:
            case ChassisLocations.RightLeg:
                return new ChassisLocations[0];
            case ChassisLocations.LeftTorso:
                return new[] {ChassisLocations.LeftArm, ChassisLocations.LeftLeg};
            case ChassisLocations.RightTorso:
                return new[] {ChassisLocations.RightArm, ChassisLocations.RightLeg};
            case ChassisLocations.CenterTorso:
                return new[] {ChassisLocations.Head, ChassisLocations.LeftTorso, ChassisLocations.RightTorso};
            default:
                throw new ArgumentException();
        }
    }

    internal static int LocationCount(ChassisLocations container)
    {
        if (container == ChassisLocations.All)
        {
            return Locations.Length;
        }
        else
        {
            return Locations.Count(location => (container & location) != ChassisLocations.None);
        }
    }

    #endregion

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
            throw new Exception("adding dynamic slots is not supported");
        }

        // find location
        if (location == ChassisLocations.None || LocationCount(location) > 1)
        {
            location = GetLocations()
                .Where(l => (l & def.AllowedLocations) != 0)
                .FirstOrDefault(l => GetLocationInfo(l).CalcMaxFree >= def.InventorySize);

            if (location == ChassisLocations.None)
            {
                return null;
            }
        }

        var locationInfo = GetLocationInfo(location);
        var overUseAtLocation = locationInfo.CalcMaxFree < def.InventorySize; // considers locational dynamic slots
        var overUseOverall = TotalFree < def.InventorySize; // considers global dynamic slots
        if (!force && (overUseAtLocation || overUseOverall))
        {
            return null;
        }

        Log.Main.Trace?.Log($"  added id={def.Description.Id} location={location} InventorySize={def.InventorySize} InventoryUsage={locationInfo.InventoryUsage} TotalInventoryUsage={TotalInventoryUsage} overUseAtLocation={overUseAtLocation} overUseOverall={overUseOverall}");

        var componentRef = new MechComponentRef(def.Description.Id, null, def.ComponentType, location);
        componentRef.DataManager = DataManager;
        componentRef.RefreshComponentDef();
        Inventory.Add(componentRef);

        Log.Main.Trace?.Log($"  adding id={def.Description.Id} location={location} InventorySize={def.InventorySize} InventoryUsage={locationInfo.InventoryUsage} TotalInventoryUsage={TotalInventoryUsage} overUseAtLocation={overUseAtLocation} overUseOverall={overUseOverall}");
        CalculateStats();
        Log.Main.Trace?.Log($"  added id={def.Description.Id} location={location} InventorySize={def.InventorySize} InventoryUsage={locationInfo.InventoryUsage} TotalInventoryUsage={TotalInventoryUsage} overUseAtLocation={overUseAtLocation} overUseOverall={overUseOverall}");

        return componentRef;
    }

    internal void Remove(MechComponentRef componentRef)
    {
        if (componentRef.Is<DynamicSlots>())
        {
            // TODO add support, doesn't work with arm actuators either
            throw new Exception("removing dynamic slots is not supported");
        }

        var def = componentRef.Def;
        var locationInfo = GetLocationInfo(componentRef.MountedLocation);
        Inventory.Remove(componentRef);

        Log.Main.Trace?.Log($"  removing id={def.Description.Id} location={componentRef.MountedLocation} InventorySize={def.InventorySize} InventoryUsage={locationInfo.InventoryUsage} TotalInventoryUsage={TotalInventoryUsage}");
        CalculateStats();
        Log.Main.Trace?.Log($"  removed id={def.Description.Id} location={componentRef.MountedLocation} InventorySize={def.InventorySize} InventoryUsage={locationInfo.InventoryUsage} TotalInventoryUsage={TotalInventoryUsage}");
    }

    private IEnumerable<ChassisLocations> GetLocations()
    {
        yield return ChassisLocations.CenterTorso;

        if (GetLocationInfo(ChassisLocations.LeftTorso).CalcMaxFree >= GetLocationInfo(ChassisLocations.RightTorso).CalcMaxFree)
        {
            yield return ChassisLocations.LeftTorso;
            yield return ChassisLocations.RightTorso;
        }
        else
        {
            yield return ChassisLocations.RightTorso;
            yield return ChassisLocations.LeftTorso;
        }

        if (GetLocationInfo(ChassisLocations.LeftLeg).CalcMaxFree >= GetLocationInfo(ChassisLocations.RightLeg).CalcMaxFree)
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

        if (GetLocationInfo(ChassisLocations.LeftArm).CalcMaxFree >= GetLocationInfo(ChassisLocations.RightArm).CalcMaxFree)
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
