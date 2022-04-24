using System.Linq;
using BattleTech.UI;
using UnityEngine;

namespace MechEngineer.Features.MechLabSlots;

internal class MechLabSlotsFixer
{
    internal static void FixSlots(WidgetLayout widgetLayout, int ___maxSlots)
    {
        // MechPropertiesWidget feature
        if (widgetLayout.widget == (widgetLayout.widget.parentDropTarget as MechLabPanel).centerTorsoWidget)
        {
            ___maxSlots = Mathf.Max(0,
                ___maxSlots - MechLabSlotsFeature.settings.TopLeftWidget.Slots -
                MechLabSlotsFeature.settings.TopRightWidget.Slots);
        }

        ModifyLayoutSlotCount(widgetLayout, ___maxSlots);
    }

    internal static void ModifyLayoutSlotCount(WidgetLayout layout, int maxSlots)
    {
        var slots = layout.slots;
        var changedSlotCount = maxSlots - slots.Count;

        if (changedSlotCount < 0)
        {
            // remove abundant
            while (slots.Count > maxSlots)
            {
                var slot = slots.Last();
                slots.RemoveAt(slots.Count - 1);
                Object.Destroy(slot.gameObject);
            }
        }
        else if (changedSlotCount > 0)
        {
            var templateSlot = slots[0];

            // add missing
            var index = slots[0].GetSiblingIndex();
            for (var i = slots.Count; i < maxSlots; i++)
            {
                var newSlot = Object.Instantiate(templateSlot, layout.layout_slots);
                //newSlot.localPosition = new Vector3(0, -(1 + i * SlotHeight), 0);
                newSlot.SetSiblingIndex(index + i);
                newSlot.name = "slot (" + i + ")";
                slots.Add(newSlot);
            }
        }
    }
}