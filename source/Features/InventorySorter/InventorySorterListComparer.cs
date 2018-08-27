using System;
using System.Collections.Generic;
using BattleTech.UI;

namespace MechEngineer
{
    public class InventorySorterListComparer : IComparer<InventoryDataObject_BASE>
    {
        private readonly Comparison<InventoryDataObject_BASE> wrapped;
        public InventorySorterListComparer(Comparison<InventoryDataObject_BASE> comparison)
        {
            wrapped = comparison;
        }

        public int Compare(InventoryDataObject_BASE a, InventoryDataObject_BASE b)
        {
            var catA = SortUtils.SortKey(a?.componentDef);
            var catB = SortUtils.SortKey(b?.componentDef);
            var val = string.Compare(catA, catB, StringComparison.Ordinal);
            return val != 0 ? val : wrapped(a, b);
        }
    }
}