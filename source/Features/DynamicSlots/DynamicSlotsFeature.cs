using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomComponents;
using MechEngineer.Features.MechLabSlots;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MechEngineer.Features.DynamicSlots
{
    internal class DynamicSlotsFeature : Feature<DynamicSlotsSettings>, IValidateMech
    {
        internal static DynamicSlotsFeature Shared = new DynamicSlotsFeature();

        internal override DynamicSlotsSettings Settings => Control.settings.DynamicSlots;

        internal static DynamicSlotsSettings settings => Shared.Settings;

        internal override void SetupFeatureLoaded()
        {
            Validator.RegisterMechValidator(CCValidation.ValidateMech, CCValidation.ValidateMechCanBeFielded);
            if (settings.DynamicSlotsValidateDropEnabled)
            {
                Validator.RegisterDropValidator(check: CCValidation.ValidateDrop);
            }
        }

        internal CCValidationAdapter CCValidation;

        private DynamicSlotsFeature()
        {
            CCValidation = new CCValidationAdapter(this);
        }

        internal void RefreshData(MechLabPanel mechLab)
        {
            var slots = new MechDefBuilder(mechLab.activeMechDef);
            using (var reservedSlots = slots.GetReservedSlots().GetEnumerator())
            {
                foreach (var location in MechDefBuilder.Locations)
                {
                    var widget = mechLab.GetLocationWidget((ArmorLocation)location); // by chance armorlocation = chassislocation for main locations
                    var adapter = new MechLabLocationWidgetAdapter(widget);
                    var used = adapter.usedSlots;
                    var start = location == ChassisLocations.CenterTorso ? MechLabSlotsFeature.settings.MechLabGeneralSlots : 0;
                    ClearFillers(widget);
                    for (var i = start; i < adapter.maxSlots; i++)
                    {
                        var slotIndex = location == ChassisLocations.CenterTorso ? i - MechLabSlotsFeature.settings.MechLabGeneralSlots : i;
                        if (i >= used && reservedSlots.MoveNext())
                        {
                            var reservedSlot = reservedSlots.Current;
                            if (reservedSlot == null)
                            {
                                throw new NullReferenceException();
                            }
                            ShowFiller(widget, slotIndex, reservedSlot);
                        }
                    }
                }
            }
        }

        public void ValidateMech(MechDef mechDef, Errors errors)
        {
            var slots = new MechDefBuilder(mechDef);
            var missing = slots.TotalMissing;
            if (missing > 0)
            {
                errors.Add(MechValidationType.InvalidInventorySlots, $"RESERVED SLOTS: Mech requires {missing} additional free slots");
            }
        }

        internal static void PrepareWidget(WidgetLayout widgetLayout)
        {
            AddFillersToSlots(widgetLayout);
        }

        internal static void ClearFillers(MechLabLocationWidget widget)
        {
            foreach (var filler in Fillers[widget.loadout.Location])
            {
                filler.Hide();
            }
        }

        internal static void ShowFiller(MechLabLocationWidget widget, int slotIndex, DynamicSlots reservedSlot)
        {
            ChassisLocations location = widget.loadout.Location;
            Fillers[location][slotIndex].Show(reservedSlot);
        }

        private static Dictionary<ChassisLocations, List<Filler>> Fillers = new Dictionary<ChassisLocations, List<Filler>>();

        private static void AddFillersToSlots(WidgetLayout layout)
        {
            var location = layout.widget.loadout.Location;
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

        private class Filler : IDisposable
        {
            private readonly GameObject gameObject;
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