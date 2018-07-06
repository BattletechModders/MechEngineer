using System.Collections.Generic;
using System.Linq;
using BattleTech;
using UnityEngine;

namespace MechEngineer
{
    internal class MechDefSlots
    {
        internal int Total { get; }
        internal int Used { get; }
        internal int Reserved { get; }
        internal List<IDynamicSlots> DynamicSlots { get; }

        internal MechDefSlots(MechDef mechDef)
        {
            Total = GetTotalSlots(mechDef.Chassis);
            Used = GetUsedSlots(mechDef.Inventory);
            DynamicSlots = GetDynamicSlots(mechDef.Inventory);
            Reserved = DynamicSlots.Sum(c => c.ReservedSlots);
        }

        internal bool IsFit => Missing == 0;
        internal bool IsOverloaded => Missing > 0;
        internal int Missing => Mathf.Max(Used + Reserved - Total, 0);

        private static int GetTotalSlots(ChassisDef chassisDef)
        {
            var total = chassisDef.LeftArm.InventorySlots;
            total += chassisDef.RightArm.InventorySlots;

            total += chassisDef.LeftLeg.InventorySlots;
            total += chassisDef.RightLeg.InventorySlots;

            total += chassisDef.RightTorso.InventorySlots;
            total += chassisDef.LeftTorso.InventorySlots;

            total += chassisDef.CenterTorso.InventorySlots;
            total += chassisDef.Head.InventorySlots;

            return total;
        }

        internal static int GetUsedSlots(IEnumerable<MechComponentRef> inventory)
        {
            return inventory.Sum(i => i.Def.InventorySize);
        }

        private static List<IDynamicSlots> GetDynamicSlots(IEnumerable<MechComponentRef> inventory)
        {
            return inventory
                .Select(i => i.Def)
                .OfType<IDynamicSlots>()
                .Distinct()
                .ToList();
        }

        internal IEnumerable<IDynamicSlots> GetReservedSlots()
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
    }
}