using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using HBS;
using UnityEngine;

namespace MechEngineer
{
    public class DynamicSlotHandler
    {
        public static DynamicSlotHandler Shared = new DynamicSlotHandler();

        #region settings
        private static readonly Color DynamicSlotsSpaceMissingColor = new Color(0.5f, 0, 0); // color changes when slots dont fit
        private static readonly ChassisLocations[] Locations = MechDefSlots.Locations; // order of locations to fill up first
        #endregion

        internal void RefreshData(MechLabPanel mechLab)
        {
            var fillerImageCache = MechLabLocationWidgetSetDataPatch.FillerImageCache;
            if (fillerImageCache.Count < Locations.Length)
            {
                return;
            }

            var slots = new MechDefSlots(mechLab.activeMechDef);
            using (var reservedSlots = slots.GetReservedSlots().GetEnumerator())
            {
                foreach (var location in Locations)
                {
                    var fillerImages = fillerImageCache[location];
                    var widget = mechLab.GetLocationWidget((ArmorLocation)location); // by chance armorlocation = chassislocation for main locations
                    var adapter = new MechLabLocationWidgetAdapter(widget);
                    var used = adapter.usedSlots;
                    for (var i = 0; i < adapter.maxSlots; i++)
                    {
                        var fillerImage = fillerImages[i];
                        if (i >= used && reservedSlots.MoveNext())
                        {
                            var reservedSlot = reservedSlots.Current;
                            if (reservedSlot == null)
                            {
                                throw new NullReferenceException();
                            }
                            fillerImage.gameObject.SetActive(true);
                            var uicolor = reservedSlot.ReservedSlotColor;
                            var color = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.GetUIColor(uicolor);
                            fillerImage.color = slots.IsOverloaded ? DynamicSlotsSpaceMissingColor : color;
                        }
                        else
                        {
                            fillerImage.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        public void ValidateMech(Dictionary<MechValidationType, List<string>> errors,
            MechValidationLevel validationLevel, MechDef mechDef)
        {
            var slots = new MechDefSlots(mechDef);
            var missing = slots.Missing;
            if (missing > 0)
            {
                errors[MechValidationType.InvalidInventorySlots]
                    .Add($"RESERVED SLOTS: Mech requires {missing} additional free slots");
            }
        }

        public bool ValidateMechCanBeFielded(MechDef mechDef)
        {
            var slots = new MechDefSlots(mechDef);
            return slots.Missing <= 0;
        }

        public string PostValidateDrop(MechLabItemSlotElement drop_item, MechDef mech, List<InvItem> new_inventory,
            List<IChange> changes)
        {
            var slots = new MechDefSlots(mech.Chassis, new_inventory.Select(x => x.item).ToList());
            var missing = slots.Missing;
            if (missing > 0)
            {
                return
                    $"Cannot add {drop_item.ComponentRef.Def.Description.Name}: Mech requires {missing} additional free slots";
            }

            return string.Empty;
        }

    }
}