using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using UnityEngine;

namespace MechEngineer
{
    internal class MechDefSlots
    {
        internal readonly int Total;
        internal readonly int Used;
        internal readonly int Reserved;
        internal readonly DynamicSlots[] DynamicSlots;

        internal readonly bool IsOverloaded;
        internal readonly int Missing;

        internal readonly ChassisDef Chassis;

        internal MechDefSlots(MechDef mechDef) : this (mechDef.Chassis, mechDef.Inventory.ToList())
        {
        }

        internal MechDefSlots(ChassisDef chassisDef, List<MechComponentRef> inventory)
        {
            Chassis = chassisDef;

            Total = Locations.Select(chassisDef.GetLocationDef).Sum(d => d.InventorySlots);
            Used = inventory.Sum(r => r.Def.InventorySize);
            DynamicSlots = inventory.Select(r => r.Def.GetComponent<DynamicSlots>()).Where(s => s != null).ToArray();
            Reserved = DynamicSlots.Sum(c => c.ReservedSlots);

            Missing = Mathf.Max(Used + Reserved - Total, 0);
            IsOverloaded = Missing > 0;

            //Control.mod.Logger.LogDebug($"Total={Total} Used={Used} Reserved={Reserved} Missing={Missing}");
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

        public override string ToString()
        {
            return $"MechDefSlots: use={Used} + res={Reserved} / tot={Total}";
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
    }
}