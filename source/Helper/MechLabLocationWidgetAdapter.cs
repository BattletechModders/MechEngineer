using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;
using TMPro;

namespace MechEngineer
{
    internal class MechLabLocationWidgetAdapter : Adapter<MechLabLocationWidget>
    {
        internal MechLabLocationWidgetAdapter(MechLabLocationWidget instance) : base(instance)
        {
        }

        internal MechLabPanel MechLab => traverse.Field("mechLab").GetValue() as MechLabPanel;
        internal TextMeshProUGUI LocationName => traverse.Field("locationName").GetValue() as TextMeshProUGUI;
        internal List<MechLabItemSlotElement> LocalInventory => traverse.Field("localInventory").GetValue() as List<MechLabItemSlotElement>;
        internal LocationLoadoutDef Loadout => instance.loadout;

        internal List<MechLabItemSlotElement> localInventory => traverse.Field("localInventory").GetValue<List<MechLabItemSlotElement>>();
        internal MechLabPanel mechLab => traverse.Field("mechLab").GetValue<MechLabPanel>();
        internal int usedSlots => traverse.Field("usedSlots").GetValue<int>();
        internal int maxSlots => traverse.Field("maxSlots").GetValue<int>();
    }
}