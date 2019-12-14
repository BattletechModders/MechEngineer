using System;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using MechEngineer.Features.MechLabSlots;
using MechLabLocationWidget_SetData_Patch = MechEngineer.Features.MechLabSlots.Patches.MechLabLocationWidget_SetData_Patch;

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
            if (MechLabSlotsFixer.Fillers.Count < MechDefBuilder.Locations.Length)
            {
                return;
            }

            var slots = new MechDefBuilder(mechLab.activeMechDef);
            using (var reservedSlots = slots.GetReservedSlots().GetEnumerator())
            {
                foreach (var location in MechDefBuilder.Locations)
                {
                    var fillers = MechLabSlotsFixer.Fillers[location];
                    var widget = mechLab.GetLocationWidget((ArmorLocation)location); // by chance armorlocation = chassislocation for main locations
                    var adapter = new MechLabLocationWidgetAdapter(widget);
                    var used = adapter.usedSlots;
                    var start = location == ChassisLocations.CenterTorso ? MechLabSlotsFeature.settings.MechLabGeneralSlots : 0;
                    for (var i = start; i < adapter.maxSlots; i++)
                    {
                        var fillerIndex = location == ChassisLocations.CenterTorso ? i - MechLabSlotsFeature.settings.MechLabGeneralSlots : i;
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