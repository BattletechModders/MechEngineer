using System.Collections.Generic;
using BattleTech;
using Localize;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.ArmorStructureRatio
{
    internal class ArmorStructureRatioFeature : Feature<ArmorStructureRatioSettings>
    {
        internal static ArmorStructureRatioFeature Shared = new ArmorStructureRatioFeature();

        internal override ArmorStructureRatioSettings Settings => Control.settings.ArmorStructureRatio;

        internal static ArmorStructureRatioSettings settings => Shared.Settings;

        public void AutoFixMechDef(MechDef mechDef)
        {
            if (!Loaded)
            {
                return;
            }

            if (mechDef.Chassis.ChassisTags.Contains(settings.IgnoreChassisTag))
            {
                return;
            }

            foreach (var location in MechDefBuilder.Locations)
            {
                ProcessMechArmorStructureRatioForLocation(mechDef, location, applyChanges:true);
            }
        }
        
        internal static bool ValidateMechArmorStructureRatio(
            MechDef mechDef,
            Dictionary<MechValidationType, List<Text>> errorMessages = null)
        {
            if (mechDef.Chassis.ChassisTags.Contains(settings.IgnoreChassisTag))
            {
                return true;
            }

            var hasInvalid = false;
            foreach (var location in MechDefBuilder.Locations)
            {

                var valid = ProcessMechArmorStructureRatioForLocation(mechDef, location, errorMessages);

                if (valid)
                {
                    continue;
                }

                hasInvalid = true;

                if (errorMessages == null)
                {
                    break;
                }
            }

            return !hasInvalid;
        }

        private static bool ProcessMechArmorStructureRatioForLocation(
            MechDef mechDef,
            ChassisLocations location,
            Dictionary<MechValidationType, List<Text>> errorMessages = null,
            bool applyChanges = false)
        {
            
            var mechLocationDef = mechDef.GetLocationLoadoutDef(location);
            var chassisLocationDef = mechDef.Chassis.GetLocationDef(location);

            var armor = Mathf.Max(mechLocationDef.AssignedArmor, 0);
            var armorRear = Mathf.Max(mechLocationDef.AssignedRearArmor, 0);

            var structure = chassisLocationDef.InternalStructure;

            var ratio = location == ChassisLocations.Head ? 3 : 2;

            var total = armor + armorRear;
            var totalMax = ratio * structure;
            
            if (total <= totalMax)
            {
                return true;
            }

            if (applyChanges)
            {
                Control.Logger.Debug?.Log($"structure={structure} location={location} totalMax={totalMax}");
                Control.Logger.Debug?.Log($"before AssignedArmor={mechLocationDef.AssignedArmor} AssignedRearArmor={mechLocationDef.AssignedRearArmor}");

                if ((location & ChassisLocations.Torso) != 0)
                {
                    mechLocationDef.AssignedArmor = PrecisionUtils.RoundUp((totalMax * 2 / 3), 5);
                    mechLocationDef.CurrentArmor = mechLocationDef.AssignedArmor;
                    mechLocationDef.AssignedRearArmor = PrecisionUtils.RoundDown((totalMax * 1 / 3), 5);
                    mechLocationDef.CurrentRearArmor = mechLocationDef.AssignedRearArmor;
                }
                else
                {
                    mechLocationDef.AssignedArmor = totalMax;
                    mechLocationDef.CurrentArmor = mechLocationDef.AssignedArmor;
                }
                
                Control.Logger.Debug?.Log($"set AssignedArmor={mechLocationDef.AssignedArmor} AssignedRearArmor={mechLocationDef.AssignedRearArmor} on location={location}");
            }

            Control.Logger.Debug?.Log($"{Mech.GetAbbreviatedChassisLocation(location)} armor={armor} armorRear={armorRear} structure={structure}");

            if (errorMessages != null)
            {
                var locationName = Mech.GetLongChassisLocation(location);
                errorMessages[MechValidationType.InvalidHardpoints].Add(new Text($"ARMOR {locationName}: Armor can only be {ratio} times more than structure."));
            }

            return false;
        }
    }
}
