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
    public class DynamicSlotHandler : IValidateMech
    {
        public static DynamicSlotHandler Shared = new DynamicSlotHandler();
        internal CCValidationAdapter CCValidation;

        public DynamicSlotHandler()
        {
            CCValidation = new CCValidationAdapter(this);
        }

        #region settings
        private static readonly Color DynamicSlotsSpaceMissingColor = new Color(0.5f, 0, 0); // color changes when slots dont fit
        private static readonly ChassisLocations[] Locations = MechDefBuilder.Locations; // order of locations to fill up first
        #endregion

        internal void RefreshData(MechLabPanel mechLab)
        {
            if (MechLabLocationWidget_SetData_Patch.Fillers.Count < Locations.Length)
            {
                return;
            }

            var slots = new MechDefBuilder(mechLab.activeMechDef);
            using (var reservedSlots = slots.GetReservedSlots().GetEnumerator())
            {
                foreach (var location in Locations)
                {
                    var fillers = MechLabLocationWidget_SetData_Patch.Fillers[location];
                    var widget = mechLab.GetLocationWidget((ArmorLocation)location); // by chance armorlocation = chassislocation for main locations
                    var adapter = new MechLabLocationWidgetAdapter(widget);
                    var used = adapter.usedSlots;
                    var start = location == ChassisLocations.CenterTorso ? Control.settings.MechLabGeneralSlots : 0;
                    for (var i = start; i < adapter.maxSlots; i++)
                    {
                        var fillerIndex = location == ChassisLocations.CenterTorso ? i - Control.settings.MechLabGeneralSlots : i;
                        var filler = fillers[fillerIndex];
                        if (i >= used && reservedSlots.MoveNext())
                        {
                            var reservedSlot = reservedSlots.Current;
                            if (reservedSlot == null)
                            {
                                throw new NullReferenceException();
                            }
                            filler.Show(reservedSlot);
                        }
                        else
                        {
                            filler.Hide();
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
    }
}