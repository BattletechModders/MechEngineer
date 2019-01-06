using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Localize;
using UnityEngine;

namespace MechEngineer
{
    public static class ArmorStructureRatioValidation
    {
        public static bool ValidateMechArmorStructureRatio(
            MechDef mechDef,
            Dictionary<MechValidationType, List<Text>> errorMessages = null)
        {
            if (!Control.settings.ArmorStructureRatioEnforcement)
            {
                return true;
            }
            
            if (Control.settings.ArmorStructureRatioEnforcementSkipMechDefs.Contains(mechDef.Description.Id))
            {
                return true;
            }

            var hasInvalid = false;
            foreach (var location in MechDefSlots.Locations)
            {

                var valid = ValidateMechArmorStructureRatioForLocation(mechDef, location, errorMessages);

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

        private static bool ValidateMechArmorStructureRatioForLocation(
            MechDef mechDef,
            ChassisLocations location,
            Dictionary<MechValidationType, List<Text>> errorMessages)
        {
            
            var mechLocationDef = mechDef.GetLocationLoadoutDef(location);
            var chassisLocationDef = mechDef.Chassis.GetLocationDef(location);

            var armor = Mathf.Max(mechLocationDef.AssignedArmor, 0);
            var armorRear = Mathf.Max(mechLocationDef.AssignedRearArmor, 0);

            var structure = chassisLocationDef.InternalStructure;

            var ratio = location == ChassisLocations.Head ? 3 : 2;

            if (armor + armorRear <= ratio * structure)
            {
                return true;
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
