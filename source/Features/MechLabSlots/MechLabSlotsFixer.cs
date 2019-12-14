using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechEngineer.Features.MechLabSlots
{
    internal class MechLabSlotsFixer
    {
        internal static void FixSlots(MechLabLocationWidget __instance, int ___maxSlots, LocationLoadoutDef ___loadout)
        {
            try
            {
                // we can't reduce to zero
                if (___maxSlots < 1)
                {
                    return;
                }

                var widgetLayout = new WidgetLayout(__instance);
                if (widgetLayout.layout_slots == null)
                {
                    return;
                }

                if (__instance == (__instance.parentDropTarget as MechLabPanel).centerTorsoWidget)
                {
                    ___maxSlots -= MechLabSlotsFeature.settings.MechLabGeneralSlots;
                }

                ModifySlotCount(widgetLayout, ___maxSlots);
                AddFillersToSlots(widgetLayout, ___loadout.Location);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        internal static void ModifySlotCount(WidgetLayout layout, int maxSlots)
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
                //newSlot.localPosition = new Vector3(0, -(1 + i * SlotHeight), 0);
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
        }

        internal static Dictionary<ChassisLocations, List<Filler>> Fillers = new Dictionary<ChassisLocations, List<Filler>>();

        private static void AddFillersToSlots(WidgetLayout layout, ChassisLocations location)
        {
            // dispose of all fillers
            if (Fillers.TryGetValue(location, out var existing))
            {
                foreach (var old in existing)
                {
                    old.Dispose();
                }
            }

            var fillers = new List<Filler>();

            foreach (var slot in layout.slots)
            {
                fillers.Add(new Filler(slot));
            }

            Fillers[location] = fillers;
        }

        internal class Filler : IDisposable
        {
            private readonly GameObject gameObject;
            private readonly MechLabItemSlotElement element;

            internal void Show(DynamicSlots.DynamicSlots dynamicSlots)
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

                if (!string.IsNullOrEmpty(dynamicSlots.BackgroundColor))
                {
                    adapter.backgroundColor.SetColorFromString(dynamicSlots.BackgroundColor);
                }

                if (dynamicSlots.ShowIcon.HasValue)
                {
                    adapter.icon.gameObject.SetActive(dynamicSlots.ShowIcon.Value);
                }

                if (dynamicSlots.ShowFixedEquipmentOverlay.HasValue)
                {
                    adapter.fixedEquipmentOverlay.gameObject.SetActive(dynamicSlots.ShowFixedEquipmentOverlay.Value);
                }

                {
                    adapter.spacers[0].SetActive(true);
                    foreach (var spacer in adapter.spacers)
                    {
                        spacer.SetActive(false);
                    }
                    element.thisRectTransform.sizeDelta = new Vector2(element.thisRectTransform.sizeDelta.x, 32f * 1);

                }
                gameObject.SetActive(true);
                element.SetDraggable(false);
            }

            internal void Hide()
            {
                gameObject.SetActive(false);
            }

            public void Dispose()
            {
                // could be null if the pool was cleared and the GameObjects were already destroyed
                // can happen if DataManager.Clear was called
                if (gameObject == null)
                {
                    return;
                }
                DataManager.PoolGameObject(MechLabPanel.MECHCOMPONENT_ITEM_PREFAB, gameObject);
            }

            internal Filler(Transform parent)
            {
                gameObject = DataManager.PooledInstantiate(
                    MechLabPanel.MECHCOMPONENT_ITEM_PREFAB,
                    BattleTechResourceType.UIModulePrefabs,
                    null, null);

                element = gameObject.GetComponent<MechLabItemSlotElement>();
                element.AllowDrag = false;
                {
                    var rect = gameObject.GetComponent<RectTransform>();
                    rect.pivot = new Vector2(0, 1);
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.anchoredPosition = Vector2.zero;
                }
                element.transform.SetParent(parent, false);
                Hide();
            }

            private static DataManager DataManager => UnityGameInstance.BattleTechGame.DataManager;
        }
    }
}