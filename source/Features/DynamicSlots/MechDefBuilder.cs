using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using CustomComponents;
using UnityEngine;

namespace MechEngineer
{
    internal class MechDefBuilder
    {
        internal readonly List<MechComponentRef> Inventory;
        
        private readonly ChassisDef Chassis;
        private readonly DataManager DataManager;
        private readonly Dictionary<ChassisLocations, int> LocationUsage = new Dictionary<ChassisLocations, int>();

        private readonly int TotalMax;
        private int TotalUsage = 0;
        internal int TotalMissing => Mathf.Max(TotalUsage + Reserved - TotalMax, 0);

        private readonly List<DynamicSlots> DynamicSlots;
        private int Reserved => DynamicSlots.Sum(c => c.ReservedSlots);
        
        internal MechDefBuilder(MechDef mechDef) : this (mechDef.Chassis, mechDef.Inventory.ToList())
        {
        }

        internal MechDefBuilder(ChassisDef chassisDef, List<MechComponentRef> inventory)
        {
            Inventory = inventory;
            
            DynamicSlots = inventory.Select(r => r.Def.GetComponent<DynamicSlots>()).Where(s => s != null).ToList();
            TotalMax = Locations.Select(chassisDef.GetLocationDef).Sum(d => d.InventorySlots);
            Chassis = chassisDef;

            DataManager = UnityGameInstance.BattleTechGame.DataManager;
            //Control.mod.Logger.LogDebug("");
            //Control.mod.Logger.LogDebug($"chassisDef={chassisDef.Description.Id}");

            foreach (var group in inventory.GroupBy(r => r.MountedLocation))
            {
                var location = group.Key;
                var sum = group.Sum(r => r.Def.InventorySize);
                LocationUsage[location] = sum;
                TotalUsage += sum;
                //Control.mod.Logger.LogDebug($"location={location} sum={sum}");
            }
        }

        internal IEnumerable<DynamicSlots> GetReservedSlots()
        {
            foreach (var reservedSlot in DynamicSlots)
            {
                for (var i = 0; i < reservedSlot.ReservedSlots; i++)
                {
                    yield return reservedSlot;
                }
            }
        }

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

        internal bool Add(MechComponentDef def, ChassisLocations location = ChassisLocations.None)
        {
            // find location
            if (location == ChassisLocations.None || LocationCount(location) > 1)
            {
                location = FindSpaceAtLocations(def.InventorySize, def.AllowedLocations);
                if (location == ChassisLocations.None)
                {
                    return false;
                }
            }
            
            TotalUsage += def.InventorySize;
            LocationUsage[location] = GetUsedSlots(location) + def.InventorySize;
            
            if (def.Is<DynamicSlots>(out var ds))
            {
                DynamicSlots.Add(ds);
            }
            
            var componentRef = new MechComponentRef(def.Description.Id, null, def.ComponentType, location);
            componentRef.DataManager = DataManager;
            componentRef.RefreshComponentDef();
            Inventory.Add(componentRef);
            return true;
        }

        internal void Remove(MechComponentRef item)
        {
            Inventory.Remove(item);
            if (item.Is<DynamicSlots>(out var ds))
            {
                DynamicSlots.Remove(ds);
            }
            LocationUsage[item.MountedLocation] -= item.Def.InventorySize;
            TotalUsage -= item.Def.InventorySize;
        }

        private ChassisLocations FindSpaceAtLocations(int slotCount, ChassisLocations allowedLocations)
        {
            return GetLocations()
                .Where(location => (location & allowedLocations) != 0)
                .FirstOrDefault(location => HasSpaceInLocation(slotCount, location));
        }

        private IEnumerable<ChassisLocations> GetLocations()
        {
            yield return ChassisLocations.CenterTorso;

            if (GetFreeSlots(ChassisLocations.LeftTorso) >= GetFreeSlots(ChassisLocations.RightTorso))
            {
                yield return ChassisLocations.LeftTorso;
                yield return ChassisLocations.RightTorso;
            }
            else
            {
                yield return ChassisLocations.RightTorso;
                yield return ChassisLocations.LeftTorso;
            }

            if (GetFreeSlots(ChassisLocations.LeftLeg) >= GetFreeSlots(ChassisLocations.RightLeg))
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

            if (GetFreeSlots(ChassisLocations.LeftArm) >= GetFreeSlots(ChassisLocations.RightArm))
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

        private bool HasSpaceInLocation(int slotCount, ChassisLocations location)
        {
            var used = GetUsedSlots(location);
            var max = GetMaxSlots(location);
            if (max - used >= slotCount)
            {
                return true;
            }
            return false;
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

        internal int GetUsedSlots(ChassisLocations location)
        {
            return LocationUsage.TryGetValue(location, out var count) ? count : 0;
        }

        internal int GetMaxSlots(ChassisLocations location)
        {
            return Chassis.GetLocationDef(location).InventorySlots;
        }

        internal int GetFreeSlots(ChassisLocations location)
        {
            return GetMaxSlots(location) - GetUsedSlots(location);
        }

        internal bool HasOveruseAtAnyLocation()
        {
            return (from location in Locations
                let max = GetMaxSlots(location)
                let used = GetUsedSlots(location)
                where used > max
                select max).Any();
        }
    }
}