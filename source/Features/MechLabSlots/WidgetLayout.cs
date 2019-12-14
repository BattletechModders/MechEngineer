using BattleTech.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots
{
    internal class WidgetLayout
    {
        internal WidgetLayout(MechLabLocationWidget widget)
        {
            this.widget = widget;
            layout_slots = widget.transform.GetChild("layout_slots");
            if (layout_slots == null)
            {
                return;
            }
                
            layout_slottedComponents = layout_slots.GetChild("layout_slottedComponents");

            slots = layout_slots.GetChildren()
                .Where(x => x.name.StartsWith("slot"))
                .OrderByDescending(x => x.localPosition.y)
                .ToList();
        }
            
        internal MechLabLocationWidget widget { get; }

        internal Transform layout_slots { get; }
        internal GridLayoutGroup layout_slots_glg => layout_slots.GetComponent<GridLayoutGroup>();

        internal Transform layout_slottedComponents { get; }
        internal VerticalLayoutGroup layout_slottedComponents_vlg => layout_slottedComponents.GetComponent<VerticalLayoutGroup>();

        internal List<Transform> slots { get; }
    }
}