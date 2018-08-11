using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineer
{
    internal class MechDefBuilder
    {
        internal readonly MechDefSlots Slots;
        internal readonly List<MechComponentRef> Inventory;
        internal readonly Dictionary<ChassisLocations, int> LocationUsage = new Dictionary<ChassisLocations, int>();

        internal MechDefBuilder(ChassisDef chassisDef, List<MechComponentRef> inventory)
        {
            Slots = new MechDefSlots(chassisDef, inventory);

            Inventory = inventory;

            //Control.mod.Logger.LogDebug("");
            //Control.mod.Logger.LogDebug($"chassisDef={chassisDef.Description.Id}");

            foreach (var group in inventory.GroupBy(r => r.MountedLocation))
            {
                var location = group.Key;
                var sum = group.Sum(r => r.Def.InventorySize);
                LocationUsage[location] = sum;
                //Control.mod.Logger.LogDebug($"location={location} sum={sum}");
            }
        }

        internal bool Add(MechComponentDef def, ChassisLocations location = ChassisLocations.None, string simGameUID = null)
        {
            // find location
            if (location == ChassisLocations.None || LocationCount(location) > 1)
            {
                location = AddSlots(def.InventorySize, def.AllowedLocations);
                if (location == ChassisLocations.None)
                {
                    return false;
                }
            }
            else
            {
                AddSlotsToLocation(def.InventorySize, location, true);
            }
            var componentRef = new MechComponentRef(def.Description.Id, simGameUID, def.ComponentType, location);
            Inventory.Add(componentRef);
            return true;
        }

        private ChassisLocations AddSlots(int slotCount, ChassisLocations allowedLocations)
        {
            return MechDefSlots.Locations
                .Where(location => (location & allowedLocations) != 0)
                .FirstOrDefault(location => AddSlotsToLocation(slotCount, location));
        }

        private bool AddSlotsToLocation(int slotCount, ChassisLocations location, bool force = false)
        {
            var used = GetUsedSlots(location);
            var max = GetMaxSlots(location);
            if (force || max - used >= slotCount)
            {
                LocationUsage[location] = used + slotCount;
                return true;
            }
            return false;
        }

        internal void Remove(MechComponentRef item)
        {
            Inventory.Remove(item);
            LocationUsage[item.MountedLocation] -= item.Def.InventorySize;
        }

        internal static int LocationCount(ChassisLocations container)
        {
            if (container == ChassisLocations.All)
            {
                return MechDefSlots.Locations.Length;
            }
            else
            {
                return MechDefSlots.Locations.Count(location => (container & location) != ChassisLocations.None);
            }
        }

        internal int GetUsedSlots(ChassisLocations location)
        {
            return LocationUsage.TryGetValue(location, out var count) ? count : 0;
        }

        internal int GetMaxSlots(ChassisLocations location)
        {
            return Slots.Chassis.GetLocationDef(location).InventorySlots;
        }

        internal int GetFreeSlots(ChassisLocations location)
        {
            return GetMaxSlots(location) - GetUsedSlots(location);
        }

        internal bool HasOveruse()
        {
            return (from location in MechDefSlots.Locations
                let max = GetMaxSlots(location)
                let used = GetUsedSlots(location)
                where used > max
                select max).Any();
        }
    }
}