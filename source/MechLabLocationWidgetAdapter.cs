using System.Collections.Generic;
using BattleTech.UI;
using DynModLib;

namespace MechEngineMod
{
    internal class MechLabLocationWidgetAdapter : Adapter<MechLabLocationWidget>
    {
        internal MechLabLocationWidgetAdapter(MechLabLocationWidget instance) : base(instance)
        {
        }

        internal List<MechLabItemSlotElement> LocalInventory
        {
            get { return traverse.Field("localInventory").GetValue() as List<MechLabItemSlotElement>; }
        }

        internal string DropErrorMessage
        {
            set { traverse.Field("dropErrorMessage").SetValue(value); }
        }
    }
}