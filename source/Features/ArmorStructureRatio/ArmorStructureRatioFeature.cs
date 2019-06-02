using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Localize;
using MechEngineer.Features.DynamicSlots;
using UnityEngine;

namespace MechEngineer.Features.ArmorStructureRatio
{
    internal class ArmorStructureRatioFeature : Feature
    {
        internal static ArmorStructureRatioFeature Shared = new ArmorStructureRatioFeature();
        internal override bool Enabled => settings?.Enabled ?? false;

        internal static Settings settings => Control.settings.ArmorStructureRatio;

        internal class Settings
        {
            public bool Enabled = true;
            public string[] SkipMechDefs = { };
        }

        public void AutoFixMechDef(MechDef mechDef)
        {
            if (!Loaded)
            {
                return;
            }

            if (settings.SkipMechDefs.Contains(mechDef.Description.Id))
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
            if (settings.SkipMechDefs.Contains(mechDef.Description.Id))
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
//                Control.mod.Logger.LogDebug($"structure={structure} location={location} totalMax={totalMax}");
//                Control.mod.Logger.LogDebug($"before AssignedArmor={mechLocationDef.AssignedArmor} AssignedRearArmor={mechLocationDef.AssignedRearArmor}");

                if ((location & ChassisLocations.Torso) != 0)
                {
                    mechLocationDef.AssignedArmor = (totalMax * 2 / 3).Round(Mathf.Ceil, 5);
                    mechLocationDef.CurrentArmor = mechLocationDef.AssignedArmor;
                    mechLocationDef.AssignedRearArmor = (totalMax * 1 / 3).Round(Mathf.Floor, 5);
                    mechLocationDef.CurrentRearArmor = mechLocationDef.AssignedRearArmor;
                }
                else
                {
                    mechLocationDef.AssignedArmor = totalMax;
                    mechLocationDef.CurrentArmor = mechLocationDef.AssignedArmor;
                }
                
                Control.mod.Logger.LogDebug($"set AssignedArmor={mechLocationDef.AssignedArmor} AssignedRearArmor={mechLocationDef.AssignedRearArmor} on location={location}");
            }
            
            //Control.mod.Logger.LogDebug($"{Mech.GetAbbreviatedChassisLocation(location)} armor={armor} armorRear={armorRear} structure={structure}");

            if (errorMessages != null)
            {
                var locationName = Mech.GetLongChassisLocation(location);
                errorMessages[MechValidationType.InvalidHardpoints].Add(new Text($"ARMOR {locationName}: Armor can only be {ratio} times more than structure."));
            }

            return false;
        }
    }
}
