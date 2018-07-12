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
            if (location == ChassisLocations.None)
            {
                location = AddSlots(def.InventorySize);
                if (location == ChassisLocations.None)
                {
                    return false;
                }
            }
            else
            {
                AddSlots(def.InventorySize, location, true);
            }
            var componentRef = new MechComponentRef(def.Description.Id, simGameUID, def.ComponentType, location);
            Inventory.Add(componentRef);
            return true;
        }

        private ChassisLocations AddSlots(int slotCount)
        {
            foreach (var location in MechDefSlots.Locations)
            {
                if (AddSlots(slotCount, location))
                {
                    return location;
                }
            }

            return ChassisLocations.None;
        }

        private bool AddSlots(int slotCount, ChassisLocations location, bool force = false)
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

        private int GetUsedSlots(ChassisLocations location)
        {
            return LocationUsage.TryGetValue(location, out var count) ? count : 0;
        }

        private int GetMaxSlots(ChassisLocations location)
        {
            return Slots.Chassis.GetLocationDef(location).InventorySlots;
        }
    }
}