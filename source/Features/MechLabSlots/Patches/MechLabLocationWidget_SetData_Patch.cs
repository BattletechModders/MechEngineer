using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabLocationWidget), "SetData")]
    public static class MechLabLocationWidget_SetData_Patch
    {
        private const int SlotHeight = 32;

        public static void Postfix(MechLabLocationWidget __instance, int ___maxSlots, LocationLoadoutDef ___loadout)
        {
            try
            {
                // we can't reduce to zero
                if (___maxSlots < 1)
                {
                    return;
                }

                var widgetLayout = new WidgetLayout(__instance, ___loadout.Location);
                if (widgetLayout.layout_slots == null)
                {
                    return;
                }

                ModifySlotCount(widgetLayout, ___maxSlots);
                AddFillersToSlots(widgetLayout);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        private static void ModifySlotCount(WidgetLayout layout, int maxSlots)
        {
            var slots = layout.slots;
            var changedSlotCount = maxSlots - slots.Count;

            if (changedSlotCount == 0)
            {
                return;
            }

            var templateSlot = slots[0];

            // add missing
            int index = slots[0].GetSiblingIndex();
            for (var i = slots.Count; i < maxSlots; i++)
            {
                var newSlot = UnityEngine.Object.Instantiate(templateSlot, layout.layout_slots);
                newSlot.localPosition = new Vector3(0, -(1 + i * SlotHeight), 0);
                newSlot.SetSiblingIndex(index + i);
                newSlot.name = "slot (" + i + ")";
                slots.Add(newSlot);
            }

            // remove abundant
            while (slots.Count > maxSlots)
            {
                var slot = slots.Last();
                slots.RemoveAt(slots.Count - 1);
                UnityEngine.Object.Destroy(slot.gameObject);
            }

            var changedHeight = changedSlotCount * SlotHeight;

            layout.widget.transform.AdjustHeight(changedHeight);
            layout.layout_slots.AdjustHeight(changedHeight);
        }

        internal static Dictionary<ChassisLocations, List<Filler>> Fillers = new Dictionary<ChassisLocations, List<Filler>>();

        private static void AddFillersToSlots(WidgetLayout layout)
        {
            var fillers = new List<Filler>();

            foreach (var slot in layout.slots)
            {
                var filler = Filler.CreateFromSlot(slot.gameObject);
                filler.Hide();
                fillers.Add(filler);
            }

            Fillers[layout.location] = fillers;
        }

        internal class Filler
        {
            private readonly GameObject layout;
            private readonly MechLabItemSlotElement element;

            internal void Show(DynamicSlots dynamicSlots)
            {
                var def = dynamicSlots.Def;
                var @ref = new MechComponentRef(def.Description.Id, null, def.ComponentType, ChassisLocations.None) {DataManager = def.DataManager};
                @ref.RefreshComponentDef();
                element.SetData(@ref, ChassisLocations.None, def.DataManager, null);
                var adapter = new MechLabItemSlotElementAdapter(element);

                if (dynamicSlots.NameText != null)
                {
                    adapter.nameText.text = dynamicSlots.NameText;
                }

                if (dynamicSlots.BonusAText == "")
                {
                    adapter.bonusTextA.gameObject.SetActive(false);
                }
                else if (dynamicSlots.BonusAText != null)
                {
                    adapter.bonusTextA.text = dynamicSlots.BonusAText;
                    adapter.bonusTextA.gameObject.SetActive(true);
                }
                
                if (dynamicSlots.BonusBText == "")
                {
                    adapter.bonusTextB.gameObject.SetActive(false);
                }
                else if (dynamicSlots.BonusBText != null)
                {
                    adapter.bonusTextB.text = dynamicSlots.BonusBText;
                    adapter.bonusTextB.gameObject.SetActive(true);
                }

                if (dynamicSlots.BackgroundColor != null)
                {
                    adapter.backgroundColor.SetUIColor(dynamicSlots.BackgroundColor.Value);
                }

                adapter.icon.gameObject.SetActive(dynamicSlots.ShowIcon);
                
                layout.SetActive(true);
                element.SetDraggable(false);
            }

            internal void Hide()
            {
                layout.SetActive(false);
            }

            internal static Filler CreateFromSlot(GameObject slot)
            {
                return new Filler(slot);
            }

            private Filler(GameObject slot)
            {
                element = slot.GetComponentInChildren<MechLabItemSlotElement>();
                if (element != null)
                {
                    layout = element.GameObject;
                    return;
                }

                layout = UnityGameInstance.BattleTechGame.DataManager.PooledInstantiate(MechLabPanel.MECHCOMPONENT_ITEM_PREFAB, BattleTechResourceType.UIModulePrefabs, null, null);
                element = layout.GetComponent<MechLabItemSlotElement>();
                element.AllowDrag = false;
                {
                    var rect = layout.GetComponent<RectTransform>();
                    rect.pivot = new Vector2(0, 1);
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.anchoredPosition = Vector2.zero;
                }
                element.transform.SetParent(slot.transform, false);
                Hide();
            }
        }

        private class WidgetLayout
        {
            internal WidgetLayout(MechLabLocationWidget widget, ChassisLocations location)
            {
                this.location = location;
                this.widget = widget;
                layout_slots = widget.transform.GetChild("layout_slots");
                if (layout_slots == null)
                {
                    return;
                }
                slots = layout_slots.GetChildren()
                    .Where(x => x.name.StartsWith("slot"))
                    .OrderByDescending(x => x.localPosition.y)
                    .ToList();
            }

            internal ChassisLocations location { get; }
            internal MechLabLocationWidget widget { get; }
            internal Transform layout_slots { get; }
            internal List<Transform> slots { get; }
        }
    }
}