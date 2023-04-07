using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomComponents;
using MechEngineer.Features.MechLabSlots;
using System;
using System.Collections.Generic;
using System.Linq;
using MechEngineer.Helper;
using MechEngineer.Misc;
using UnityEngine;

namespace MechEngineer.Features.DynamicSlots;

internal class DynamicSlotsFeature : Feature<DynamicSlotsSettings>, IValidateMech
{
    internal static readonly DynamicSlotsFeature Shared = new();

    // TODO move settings instances to features and allow loading them one by one
    // then AutoFixSettings depending on DynamicSlotsSettings will be resolved properly
    internal override DynamicSlotsSettings Settings => Control.Settings?.DynamicSlots ?? new DynamicSlotsSettings();

    internal static DynamicSlotsSettings settings => Shared.Settings;

    protected override void SetupFeatureLoaded()
    {
        Validator.RegisterMechValidator(CCValidation.ValidateMech, CCValidation.ValidateMechCanBeFielded);
        if (settings.DynamicSlotsValidateDropEnabled)
        {
            Validator.RegisterDropValidator(check: CCValidation.ValidateDrop);
        }
    }

    private readonly CCValidationAdapter CCValidation;

    private DynamicSlotsFeature()
    {
        CCValidation = new CCValidationAdapter(this);
    }

    private class DynamicSlotBuilder : IComparable<DynamicSlotBuilder>
    {
        private readonly ChassisLocations location;
        private readonly int maxSlots;
        internal readonly MechLabLocationWidget widget;
        internal int currentFreeSlots;
        internal int fixedSlots;

        internal DynamicSlotBuilder(
            MechDefBuilder builder,
            ChassisLocations location,
            MechLabLocationWidget widget)
        {
            this.location = location;
            this.widget = widget;

            var locationInfo = builder.LocationUsages[location];
            currentFreeSlots = locationInfo.FreeIncludingDynamic;
            fixedSlots = locationInfo.Fixed;

            maxSlots = widget.maxSlots;
        }

        int IComparable<DynamicSlotBuilder>.CompareTo(DynamicSlotBuilder other)
        {
            var diff = currentFreeSlots - other.currentFreeSlots;
            if (diff != 0)
            {
                return diff;
            }
            var index = Array.IndexOf(settings.LocationPriorityOrder, location);
            var otherIndex = Array.IndexOf(settings.LocationPriorityOrder, other.location);
            return -index.CompareTo(otherIndex);
        }

        internal bool currentFreeSlotFixed => maxSlots - currentFreeSlots < fixedSlots;

        internal int currentFreeSlotIndex
        {
            get
            {
                var index = maxSlots - currentFreeSlots;
                if (location == ChassisLocations.CenterTorso)
                {
                    index -= MechLabSlotsFeature.settings.TopLeftWidget.Slots;
                    index -= MechLabSlotsFeature.settings.TopRightWidget.Slots;
                }
                return index;
            }
        }
    }

    internal void RefreshData(MechLabPanel mechLab)
    {
        var builder = new MechDefBuilder(mechLab.CreateMechDef());
        var fslList = new List<DynamicSlotBuilder>();
        var fslDict = new Dictionary<ChassisLocations, DynamicSlotBuilder>();
        foreach (var location in LocationUtils.Locations)
        {
            // armorlocation = chassislocation for main locations
            var widget = mechLab.GetLocationWidget((ArmorLocation)location);
            ClearFillers(widget);
            var dsbuilder = new DynamicSlotBuilder(builder, location, widget);
            fslList.Add(dsbuilder);
            fslDict.Add(location, dsbuilder);
        }

        // first pass for items with location restrictions
        foreach (var location in LocationUtils.DynamicLocationalOrder)
        {
            foreach (var componentRef in builder.Inventory)
            {
                if (componentRef.MountedLocation != location)
                {
                    continue;
                }
                var slot = componentRef.Def.GetComponent<DynamicSlots>();
                if (slot is not { InnerAdjacentOnly: true })
                {
                    continue;
                }
                var reservedSlots = slot.ReservedSlots;
                {
                    var fsl = fslDict[location];
                    while (fsl.currentFreeSlots > 0 && reservedSlots > 0)
                    {
                        ShowFiller(fsl.widget, slot, fsl.currentFreeSlotIndex, fsl.currentFreeSlotFixed);
                        fsl.currentFreeSlots--;
                        reservedSlots--;
                    }
                }
                if (reservedSlots <= 0)
                {
                    continue;
                }
                var innerLocation = LocationUtils.GetInnerAdjacentLocation(location);
                if (innerLocation == ChassisLocations.None)
                {
                    continue;
                }
                {
                    var fsl = fslDict[innerLocation];
                    while (fsl.currentFreeSlots > 0 && reservedSlots > 0)
                    {
                        ShowFiller(fsl.widget, slot, fsl.currentFreeSlotIndex, true);
                        fsl.currentFreeSlots--;
                        reservedSlots--;
                    }
                }
            }
        }

        // second pass dynamic slots without location restrictions
        foreach (var componentRef in builder.Inventory)
        {
            var slot = componentRef.Def.GetComponent<DynamicSlots>();
            if (slot == null || slot.InnerAdjacentOnly)
            {
                continue;
            }

            var reservedSlots = slot.ReservedSlots;
            while (reservedSlots > 0)
            {
                var fsl = fslList.Max();
                if (fsl.currentFreeSlots <= 0)
                {
                    return; // no free slots left to use if thats the max
                }
                ShowFiller(fsl.widget, slot, fsl.currentFreeSlotIndex, fsl.currentFreeSlotFixed);
                fsl.currentFreeSlots--;
                reservedSlots--;
            }
        }
    }

    public void ValidateMech(MechDef mechDef, Errors errors)
    {
        var builder = new MechDefBuilder(mechDef);
        builder.HasOveruseAtAnyLocation(errors);
    }

    internal static void PrepareWidget(WidgetLayout widgetLayout)
    {
        AddFillersToSlots(widgetLayout);
    }

    internal static void ClearFillers(MechLabLocationWidget widget)
    {
        if (Fillers.TryGetValue(widget.loadout.Location, out var fillers))
        {
            foreach (var filler in fillers)
            {
                filler.Reset();
            }
        }
    }

    internal static void ShowFiller(MechLabLocationWidget widget, DynamicSlots slots, int slotIndex, bool isReservedSlot)
    {
        var location = widget.loadout.Location;
        Fillers[location][slotIndex].Show(slots, isReservedSlot);
    }

    private static Dictionary<ChassisLocations, List<Filler>> Fillers = new();

    private static void AddFillersToSlots(WidgetLayout layout)
    {
        var location = layout.widget.loadout.Location;

        if (!Fillers.TryGetValue(location, out var fillers))
        {
            fillers = new List<Filler>();
            Fillers[location] = fillers;
        }

        // remove already destroyed fillers
        for (var index = fillers.Count - 1; index >= 0; index--)
        {
            if (fillers[index].IsInvalid())
            {
                fillers.RemoveAt(index);
            }
        }

        var existingLastIndex = fillers.Count - 1;
        var newLastIndex = layout.slots.Count - 1;

        // only dispose/new fillers when needed, saves 600ms on my machine on pressing refit
        if (existingLastIndex > newLastIndex)
        {
            var superfluousCount = existingLastIndex - newLastIndex;
            foreach (var superfluous in fillers.GetRange(newLastIndex, superfluousCount))
            {
                superfluous.Dispose();
            }
            fillers.RemoveRange(newLastIndex, superfluousCount);
        }
        else
        {
            for (var newIndex = existingLastIndex + 1; newIndex <= newLastIndex; newIndex++)
            {
                fillers.Add(new Filler());
            }
        }

        for (var index = 0; index <= newLastIndex; index++)
        {
            fillers[index].Update(layout.slots[index]);
        }
    }

    private class Filler : IDisposable
    {
        private GameObject gameObject;
        private readonly MechLabItemSlotElement element;
        private readonly RectTransform backgroundsRect;

        internal void Show(DynamicSlots slots, bool isReservedSlot)
        {
            var dataManager = UnityGameInstance.BattleTechGame.DataManager;
            var id = slots.Def.Description.Id;
            var type = slots.Def.ComponentType;
            var @ref = new MechComponentRef(id, null, type, ChassisLocations.None) {DataManager = dataManager};
            @ref.RefreshComponentDef();
            element.SetData(@ref, ChassisLocations.None, dataManager, null);

            slots.ApplyTo(element, isReservedSlot);

            // support template elements larger 1
            element.ClearSpacers();
            element.thisRectTransform.sizeDelta = new Vector2(element.thisRectTransform.sizeDelta.x, 32f);

            if (isReservedSlot)
            {
                ResetSolidBorder();
            }
            else
            {
                HideSolidBorder();
            }

            gameObject.SetActive(true);
            element.SetDraggable(false);
        }

        internal void Reset()
        {
            if (IsInvalid())
            {
                return;
            }

            ResetSolidBorder();
            gameObject.SetActive(false);
        }

        private void HideSolidBorder()
        {
            backgroundsRect.offsetMin = new Vector2(1, 1);
            backgroundsRect.offsetMax = new Vector2(-1, -1);
        }
        private void ResetSolidBorder()
        {
            backgroundsRect.offsetMin = new Vector2(0, 0);
            backgroundsRect.offsetMax = new Vector2(0, 0);
        }

        public bool IsInvalid()
        {
            return gameObject == null;
        }

        public void Dispose()
        {
            // could be null if the pool was cleared
            if (IsInvalid())
            {
                return;
            }

            Reset();
            DataManager.PoolGameObject(MechLabPanel.MECHCOMPONENT_ITEM_PREFAB, gameObject);
            gameObject = null!;
        }

        public void Update(Transform parent)
        {
            element.transform.SetParent(parent, false);
            Reset();
        }

        internal Filler()
        {
            gameObject = DataManager.PooledInstantiate(
                MechLabPanel.MECHCOMPONENT_ITEM_PREFAB,
                BTLoadUtils.GetResourceType(nameof(BattleTechResourceType.UIModulePrefabs)));


            {
                element = gameObject.GetComponent<MechLabItemSlotElement>();
                element.AllowDrag = false;
                {
                    var rect = gameObject.GetComponent<RectTransform>();
                    rect.pivot = new Vector2(0, 1);
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.anchoredPosition = Vector2.zero;
                }
            }

            {
                var rep = gameObject.transform.Find("Representation");
                var layoutComponents = rep.Find("layout_component");
                var backgrounds = layoutComponents.Find("BACKGROUNDS");
                backgroundsRect = backgrounds.GetComponent<RectTransform>();
            }
        }

        private static DataManager DataManager => UnityGameInstance.BattleTechGame.DataManager;
    }
}
