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
            var builder = new MechDefBuilder(mechLab.activeMechDef);
            var totalFree = builder.TotalFree;

            foreach (var location in MechDefBuilder.Locations)
            {
                 // armorlocation = chassislocation for main locations
                var widget = mechLab.GetLocationWidget((ArmorLocation)location);
                ClearFillers(widget);

                var adapter = new MechLabLocationWidgetAdapter(widget);
                var used = adapter.usedSlots;
                var max = adapter.maxSlots;
                var free = Mathf.Max(max - used, 0);

                if (free == 0)
                {
                    continue;
                }

                var dynUsage = free - totalFree;
                if (dynUsage <= 0)
                {
                    continue;
                }

                var start = used;
                if (location == ChassisLocations.CenterTorso)
                {
                    start += MechLabSlotsFeature.settings.MechLabGeneralSlots;
                }

                for (var i = start; i < used + dynUsage; i++)
                {
                    var slotIndex = location == ChassisLocations.CenterTorso ? i - MechLabSlotsFeature.settings.MechLabGeneralSlots : i;
                    ShowFiller(widget, slotIndex);
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

        internal static void ShowFiller(MechLabLocationWidget widget, int slotIndex)
        {
            ChassisLocations location = widget.loadout.Location;
            Fillers[location][slotIndex].Show();
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

            internal void Show()
            {
                var dataManager = UnityGameInstance.BattleTechGame.DataManager;
                var id = settings.DynamicSlotComponentId;
                var type = settings.DynamicSlotComponentType;
                var @ref = new MechComponentRef(id, null, type, ChassisLocations.None) {DataManager = dataManager};
                @ref.RefreshComponentDef();
                element.SetData(@ref, ChassisLocations.None, dataManager, null);
                
                if (settings.ShowFixedEquipmentOverlay)
                {
                    var adapter = new MechLabItemSlotElementAdapter(element);
                    adapter.fixedEquipmentOverlay.gameObject.SetActive(true);
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