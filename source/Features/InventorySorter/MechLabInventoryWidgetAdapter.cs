using System;
using BattleTech.UI;

namespace MechEngineer
{
    public class MechLabInventoryWidgetAdapter : Adapter<MechLabInventoryWidget>
    {
        public MechLabInventoryWidgetAdapter(MechLabInventoryWidget instance) : base(instance)
        {
        }

        public Comparison<InventoryItemElement_NotListView> currentSort
        {
            get => traverse.Field<Comparison<InventoryItemElement_NotListView>>(nameof(currentSort)).Value;
            set => traverse.Field<Comparison<InventoryItemElement_NotListView>>(nameof(currentSort)).Value = value;
        }

        public bool invertSort
        {
            get => traverse.Field<bool>(nameof(invertSort)).Value;
            set => traverse.Field<bool>(nameof(invertSort)).Value = value;
        }
    }
}