using System;
using System.Collections.Generic;
using BattleTech.UI;

namespace MechEngineer
{
    public class MechLabInventoryWidget_ListViewAdapter : Adapter<MechLabInventoryWidget_ListView>
    {
        public MechLabInventoryWidget_ListViewAdapter(MechLabInventoryWidget_ListView instance) : base(instance)
        {
        }

        public IComparer<InventoryDataObject_BASE> currentListItemSorter
        {
            get => traverse.Field<IComparer<InventoryDataObject_BASE>>(nameof(currentListItemSorter)).Value;
            set => traverse.Field<IComparer<InventoryDataObject_BASE>>(nameof(currentListItemSorter)).Value = value;
        }

        public Comparison<InventoryDataObject_BASE> currentSort
        {
            get => traverse.Field<Comparison<InventoryDataObject_BASE>>(nameof(currentSort)).Value;
            set => traverse.Field<Comparison<InventoryDataObject_BASE>>(nameof(currentSort)).Value = value;
        }

        public bool invertSort
        {
            get => traverse.Field<bool>(nameof(invertSort)).Value;
            set => traverse.Field<bool>(nameof(invertSort)).Value = value;
        }
    }
}