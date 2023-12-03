using System.Linq;
using BattleTech;
using BattleTech.UI;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.OverrideTonnage;
using MechEngineer.Helper;
using UnityEngine;

namespace MechEngineer.Features.MechLabSlots;

internal static class MechLabSlotsFixer
{
    private static GameObject? s_slotTemplate;

    internal static void FixWidgetSlotsActiveNamingFillers(MechLabLocationWidget widget)
    {
        var widgetLayout = new WidgetLayout(widget);

        if (s_slotTemplate == null)
        {
            s_slotTemplate = Object.Instantiate(widgetLayout.slots[0].gameObject, SharedGameObjects.ContainerTransform);
            s_slotTemplate.SetActive(false); // everything from pool should already be deactivated
        }

        var chassisDef = widget.mechLab.activeMechDef.Chassis;
        var locationDef = widget.chassisLocationDef;

        var maxSlots = widget.maxSlots;

        if (locationDef.Location == ChassisLocations.CenterTorso)
        {
            // MechPropertiesWidget feature
            // a bit much being exposed
            CustomWidgetsFixMechLab.GetCustomWidgetsAndSlots(
                chassisDef,
                out var topLeftSettings,
                out var topRightSettings,
                out var topLeftWidget,
                out var topRightWidget
            );
            maxSlots -= topLeftSettings.Slots + topRightSettings.Slots;

            ModifyLayoutSlots(new WidgetLayout(topLeftWidget), topLeftSettings.Slots);
            topLeftWidget.gameObject.SetActive(topLeftSettings.Slots > 0);
            topLeftWidget.locationName.SetText(topLeftSettings.Label);

            // duplication
            ModifyLayoutSlots(new WidgetLayout(topRightWidget), topRightSettings.Slots);
            topRightWidget.gameObject.SetActive(topRightSettings.Slots > 0);
            topRightWidget.locationName.SetText(topRightSettings.Label);
        }

        ModifyLayoutSlots(widgetLayout, maxSlots);
        widget.gameObject.SetActive(locationDef.InventorySlots > 0 && !LegacyShouldHide(locationDef));
        var text = ChassisLocationNamingUtils.GetLocationLabel(chassisDef, locationDef.Location);
        widget.locationName.SetText(text);

        DynamicSlotsFeature.PrepareFillerSlots(widgetLayout);
    }

    private static bool LegacyShouldHide(LocationDef def)
    {
        // old way of hiding
        // hide any location with maxArmor <= 0 && structure <= 1
        // for vehicles and troopers
        return PrecisionUtils.SmallerOrEqualsTo(def.MaxArmor, 0)
               && PrecisionUtils.SmallerOrEqualsTo(def.InternalStructure, 1);
    }

    private static void ModifyLayoutSlots(WidgetLayout layout, int maxSlots)
    {
        var slots = layout.slots;
        maxSlots = Mathf.Max(maxSlots, 1);
        var changedSlotCount = maxSlots - slots.Count;

        if (changedSlotCount < 0)
        {
            // remove abundant
            while (slots.Count > maxSlots)
            {
                var slot = slots.Last();
                slots.RemoveAt(slots.Count - 1);
                slot.gameObject.SetActive(false);
                Object.Destroy(slot.gameObject);
            }
        }
        else if (changedSlotCount > 0)
        {
            // add missing
            var index = slots[0].GetSiblingIndex();
            for (var i = slots.Count; i < maxSlots; i++)
            {
                var newSlot = Object.Instantiate(s_slotTemplate!, layout.layout_slots);
                //newSlot.localPosition = new Vector3(0, -(1 + i * SlotHeight), 0);
                newSlot.transform.SetSiblingIndex(index + i);
                newSlot.name = "slot (" + i + ")";
                newSlot.SetActive(true);
                slots.Add(newSlot.transform);
            }
        }
    }
}