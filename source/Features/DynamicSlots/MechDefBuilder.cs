using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using CustomComponents;
using UnityEngine;

namespace MechEngineer.Features.DynamicSlots
{
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

            foreach (var group in Inventory.GroupBy(r => r.MountedLocation))
            {
                var location = group.Key;

                var max = GetMaxSlots(location);
                var sum = group.Sum(r => r.Def.InventorySize);

                SetInventoryUsedSlots(location, sum);
                TotalInventoryUsage += sum;
            }

            TotalDynamicGlobalUsage = Inventory
                .Select(r => r.Def.GetComponent<DynamicSlots>())
                .Where(s => s != null && !s.InnerAdjacentOnly)
                .Sum(s => s.ReservedSlots);

            // "quick" version for validation only
            foreach (var componentRef in Inventory
                .Where(r => r.Def.Is<DynamicSlots>(out var ds) && ds.InnerAdjacentOnly)
                .OrderBy(r => Array.IndexOf(RestrictedDynamicSlotsOrder, r.MountedLocation))
                .ThenBy(r => r.ComponentDefID))
            {
                var slot = componentRef.Def.GetComponent<DynamicSlots>();
                int useSpace(int required, ChassisLocations location)
                {
                    if (location != ChassisLocations.None)
                    {
                        var max = GetMaxSlots(location);
                        var usage = GetPreferredUsedSlots(location);

                        var free = max - usage;
                        if (free > 0)
                        {
                            var take = Mathf.Min(free, required);
                            required -= take;
                            TotalDynamicLocationalUsage += take;
                            SetDynamicLocationalPreferredMaxUsedSlots(location, GetDynamicLocationalPreferredMaxUsedSlots(location) + take);
                        }
                    }
                    return required;
                }
                var requiredOnLocation = useSpace(slot.ReservedSlots, componentRef.MountedLocation);
                if (requiredOnLocation > 0)
                {
                    var location = GetNearestAdjacentLocation(componentRef.MountedLocation);
                    requiredOnLocation = useSpace(requiredOnLocation, location);
                }
                if (requiredOnLocation > 0)
                {
                    TotalDynamicLocationalMissing += requiredOnLocation;
                }
            }

            Control.mod.Logger.LogDebug(this);
        }

        #region counting stats and functions

        // precedence order: 1. inventory size, 2. locational restricted dynamic slots, 3. fully movable dynamic slots

        private int TotalMax;
        private int TotalInventoryUsage;
        private int TotalDynamicGlobalUsage;
        private int TotalDynamicLocationalUsage;
        private int TotalDynamicLocationalMissing;
        private int TotalUsage => TotalInventoryUsage + TotalDynamicGlobalUsage + TotalDynamicLocationalUsage;
        internal int TotalMissing => Mathf.Max(TotalUsage - TotalMax, 0) + TotalDynamicLocationalMissing;
        private int TotalFree => Mathf.Max(TotalMax - TotalUsage, 0);

        public override string ToString()
        {
            return $"ChassisId={Chassis.Description.Id} TotalInventoryUsage={TotalInventoryUsage} TotalDynamicGlobalUsage={TotalDynamicGlobalUsage}" +
                $" TotalDynamicLocationRestrictedUsage={TotalDynamicLocationalUsage} TotalDynamicLocationRestrictedMissing={TotalDynamicLocationalMissing}";
        }

        // always without dynamic global but does include possible removal of locational dynamic slots
        private Dictionary<ChassisLocations, int> LocationalPreferredMaxMovable;
        internal int GetLocationalPreferredMaxMovable(ChassisLocations location)
        {
            if (LocationalPreferredMaxMovable == null) // lazy computation
            {
                LocationalPreferredMaxMovable = new Dictionary<ChassisLocations, int>();
                CalcLocationalMaxMovableSlots();
            }
            return LocationalPreferredMaxMovable.TryGetValue(location, out var count) ? count : 0;
        }
        private void SetLocationalPreferredMaxMovable(ChassisLocations location, int count)
        {
            LocationalPreferredMaxMovable[location] = count;
        }
        private void CalcLocationalMaxMovableSlots()
        {
            foreach (var location in RestrictedDynamicSlotsOrder.Reverse())
            {
                var adjacentLocation = GetNearestAdjacentLocation(location);
                if (adjacentLocation == ChassisLocations.None)
                {
                    continue;
                }
                var maxUsedSlots = GetDynamicLocationalPreferredMaxUsedSlots(location);
                var adjacentFree = GetPreferredFreeSlots(adjacentLocation);
                var adjacentMaxMovable = GetLocationalPreferredMaxMovable(adjacentLocation);
                var maxFree = Mathf.Min(adjacentFree + adjacentMaxMovable, maxUsedSlots);
                SetLocationalPreferredMaxMovable(location, maxFree);
            }
        }

        // used to get if a slot is movable or not
        // fixed = inventory and minimum location and minimum global slots
        internal int GetMiumumFixedSlots(ChassisLocations location)
        {
            var inventory = GetInventoryUsedSlots(location);
            var minimumLocational = GetDynamicLocationalPreferredMaxUsedSlots(location) - GetLocationalPreferredMaxMovable(location);
            var max = GetMaxSlots(location);
            var globalFree = TotalMax - TotalUsage;
            var invLocFree = max - inventory - minimumLocational;
            var free = Mathf.Min(globalFree, invLocFree);
            return max - free;
        } 

        private readonly Dictionary<ChassisLocations, int> LocationInventoryUsage = new Dictionary<ChassisLocations, int>();
        internal int GetInventoryUsedSlots(ChassisLocations location)
        {
            return LocationInventoryUsage.TryGetValue(location, out var count) ? count : 0;
        }

        private void SetInventoryUsedSlots(ChassisLocations location, int count)
        {
            LocationInventoryUsage[location] = count;
        }

        // location restricted usage
        private readonly Dictionary<ChassisLocations, int> DynamicLocationalPreferredMaxUsage = new Dictionary<ChassisLocations, int>();
        private int GetDynamicLocationalPreferredMaxUsedSlots(ChassisLocations location)
        {
            return DynamicLocationalPreferredMaxUsage.TryGetValue(location, out var count) ? count : 0;
        }

        private void SetDynamicLocationalPreferredMaxUsedSlots(ChassisLocations location, int count)
        {
            DynamicLocationalPreferredMaxUsage[location] = count;
        }

        internal int GetMaxSlots(ChassisLocations location)
        {
            return Chassis.GetLocationDef(location).InventorySlots;
        }

        // preferred: maximize locational dynamic (and omits global dynamic)
        private int GetPreferredUsedSlots(ChassisLocations location)
        {
            return GetInventoryUsedSlots(location) + GetDynamicLocationalPreferredMaxUsedSlots(location);
        }

        private int GetPreferredFreeSlots(ChassisLocations location)
        {
            return GetMaxSlots(location) - GetPreferredUsedSlots(location);
        }

        // does not support locational dynamic slots
        internal bool HasOveruseAtAnyLocation()
        {
            return (from location in Locations
                    let max = GetMaxSlots(location)
                    let used = GetPreferredUsedSlots(location)
                    where used > max
                    select max).Any();
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
            ChassisLocations.RightArm,
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
            ChassisLocations.CenterTorso,
        };

        internal static ChassisLocations GetNearestAdjacentLocation(ChassisLocations location)
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
                case ChassisLocations.CenterTorso:
                default:
                    return ChassisLocations.None;
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

        internal bool Add(MechComponentDef def, ChassisLocations location = ChassisLocations.None, bool force = false)
        {
            if (def.Is<DynamicSlots>())
            {
                // TODO add support, doesn't work with arm actuators either
                throw new Exception("adding dynamic slots is not supported");
            }

            // find location
            if (location == ChassisLocations.None || LocationCount(location) > 1)
            {
                location = FindSpaceAtLocations(def.InventorySize, def.AllowedLocations);
                if (location == ChassisLocations.None)
                {
                    return false;
                }
            }
            
            var newLocationUsage = GetPreferredUsedSlots(location) + def.InventorySize;
            if (!force && newLocationUsage > GetMaxSlots(location))
            {
                return false;
            }
            
            TotalInventoryUsage += def.InventorySize;
            SetInventoryUsedSlots(location, newLocationUsage);
            
            var componentRef = new MechComponentRef(def.Description.Id, null, def.ComponentType, location);
            componentRef.DataManager = DataManager;
            componentRef.RefreshComponentDef();
            Inventory.Add(componentRef);
            return true;
        }

        internal void Remove(MechComponentRef item)
        {
            if (item.Is<DynamicSlots>())
            {
                // TODO add support, doesn't work with arm actuators either
                throw new Exception("removing dynamic slots is not supported");
            }

            Inventory.Remove(item);
            SetInventoryUsedSlots(item.MountedLocation, GetPreferredUsedSlots(item.MountedLocation) - item.Def.InventorySize);
            TotalInventoryUsage -= item.Def.InventorySize;
        }

        private ChassisLocations FindSpaceAtLocations(int slotCount, ChassisLocations allowedLocations)
        {
            return GetLocations()
                .Where(location => (location & allowedLocations) != 0)
                .FirstOrDefault(location => HasSpaceInLocation(slotCount, location));
        }

        private bool HasSpaceInLocation(int slotCount, ChassisLocations location)
        {
            var used = GetPreferredUsedSlots(location);
            var max = GetMaxSlots(location);
            if (max - used >= slotCount)
            {
                return true;
            }
            return false;
        }

        private IEnumerable<ChassisLocations> GetLocations()
        {
            yield return ChassisLocations.CenterTorso;

            if (GetPreferredFreeSlots(ChassisLocations.LeftTorso) >= GetPreferredFreeSlots(ChassisLocations.RightTorso))
            {
                yield return ChassisLocations.LeftTorso;
                yield return ChassisLocations.RightTorso;
            }
            else
            {
                yield return ChassisLocations.RightTorso;
                yield return ChassisLocations.LeftTorso;
            }

            if (GetPreferredFreeSlots(ChassisLocations.LeftLeg) >= GetPreferredFreeSlots(ChassisLocations.RightLeg))
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

            if (GetPreferredFreeSlots(ChassisLocations.LeftArm) >= GetPreferredFreeSlots(ChassisLocations.RightArm))
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
}