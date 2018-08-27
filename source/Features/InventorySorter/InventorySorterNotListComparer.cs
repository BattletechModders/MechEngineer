using System;
using System.Collections.Generic;
using BattleTech.UI;

namespace MechEngineer
{
    public class InventorySorterNotListComparer : IComparer<InventoryItemElement_NotListView>
    {
        private readonly Comparison<InventoryItemElement_NotListView> wrapped;
        public InventorySorterNotListComparer(Comparison<InventoryItemElement_NotListView> comparison)
        {
            wrapped = comparison;
        }

        public int Compare(InventoryItemElement_NotListView a, InventoryItemElement_NotListView b)
        {
            var catA = SortUtils.SortKey(a?.ComponentRef?.Def);
            var catB = SortUtils.SortKey(b?.ComponentRef?.Def);
            var val = string.Compare(catA, catB, StringComparison.Ordinal);
            return val != 0 ? val : wrapped(a, b);
        }
    }
}