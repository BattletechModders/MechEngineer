using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Localize;
using MechEngineer.Features.ArmorMaximizer;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.ArmorStructureRatio;

internal class ArmorStructureRatioFeature : Feature<ArmorStructureRatioSettings>
{
    internal static readonly ArmorStructureRatioFeature Shared = new();

    internal override ArmorStructureRatioSettings Settings => Control.settings.ArmorStructureRatio;

    internal static ArmorStructureRatioSettings settings => Shared.Settings;

    public void AutoFixChassisDef(ChassisDef chassisDef)
    {
        if (!Loaded)
        {
            return;
        }

        if (chassisDef.ChassisTags.Contains(settings.IgnoreChassisTag))
        {
            return;
        }

        for (var index = 0; index < chassisDef.Locations.Length; index++)
        {
            var locationDef = chassisDef.Locations[index];
            var max = GetMaximumArmorPoints(locationDef);
            float front, back;
            if ((locationDef.Location & ChassisLocations.Torso) != ChassisLocations.None)
            {
                front = back = max;
            }
            else
            {
                front = max;
                back = 0;
            }
            // TODO remove readonly via publicizer to avoid copying via constructor
            chassisDef.Locations[index] = new LocationDef(
                locationDef.Hardpoints,
                locationDef.Location,
                locationDef.Tonnage,
                locationDef.InventorySlots,
                front,
                back,
                locationDef.InternalStructure
            );
        }
        chassisDef.refreshLocationReferences();
    }

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
            ProcessMechArmorStructureRatioForLocation(mechDef, location, applyChanges: true);
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

        var total = armor + armorRear;
        var totalMax = GetMaximumArmorPoints(chassisLocationDef);

        // only fix over-allocated locations
        if (total <= totalMax)
        {
            return true;
        }

        if (applyChanges)
        {
            Control.Logger.Trace?.Log($"structure={structure} location={location} total={total} totalMax={totalMax}");
            Control.Logger.Trace?.Log($"before AssignedArmor={mechLocationDef.AssignedArmor} AssignedRearArmor={mechLocationDef.AssignedRearArmor}");

            if ((location & ChassisLocations.Torso) != 0)
            {
                var ratio = ArmorAllocationRatioFrontRear(location);
                mechLocationDef.AssignedArmor = PrecisionUtils.RoundDown(totalMax * ratio, ArmorPerStep);
                mechLocationDef.CurrentArmor = mechLocationDef.AssignedArmor;
                mechLocationDef.AssignedRearArmor = totalMax - mechLocationDef.AssignedArmor;
                mechLocationDef.CurrentRearArmor = mechLocationDef.AssignedRearArmor;
            }
            else
            {
                mechLocationDef.AssignedArmor = totalMax;
                mechLocationDef.CurrentArmor = mechLocationDef.AssignedArmor;
            }

            Control.Logger.Trace?.Log($"set AssignedArmor={mechLocationDef.AssignedArmor} AssignedRearArmor={mechLocationDef.AssignedRearArmor} on location={location}");
        }

        Control.Logger.Trace?.Log($"{Mech.GetAbbreviatedChassisLocation(location)} armor={armor} armorRear={armorRear} structure={structure}");

        if (errorMessages != null)
        {
            var locationName = Mech.GetLongChassisLocation(location);
            errorMessages[MechValidationType.InvalidHardpoints].Add(new Text($"ARMOR {locationName}: Armor can only be {GetArmorToStructureRatio(location)} times more than structure."));
        }

        return false;
    }

    internal static int GetMaximumArmorPoints(MechDef mechDef)
    {
        return MechDefBuilder.Locations
            .Select(location => mechDef.Chassis.GetLocationDef(location))
            .Select(GetMaximumArmorPoints)
            .Sum();
    }

    internal static int GetMaximumArmorPoints(LocationDef locationDef)
    {
        var ratio = GetArmorToStructureRatio(locationDef.Location);
        return (int)PrecisionUtils.RoundDown(locationDef.InternalStructure * ratio, ArmorPerStep);
    }

    internal static int ArmorPerStep => (int)UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_STEP;

    private static float ArmorAllocationRatioFrontRear(ChassisLocations location)
    {
        var constants = UnityGameInstance.BattleTechGame.MechStatisticsConstants;
        float front, back;
        if (location == ChassisLocations.LeftTorso)
        {
            front = constants.ArmorAllocationRatioLeftTorso;
            back = constants.ArmorAllocationRatioLeftTorsoRear;
        }
        else if (location == ChassisLocations.RightTorso)
        {
            front = constants.ArmorAllocationRatioRightTorso;
            back = constants.ArmorAllocationRatioRightTorsoRear;
        }
        else
        {
            front = constants.ArmorAllocationRatioCenterTorso;
            back = constants.ArmorAllocationRatioCenterTorsoRear;
        }
        return front / back;
    }

    private static int GetArmorToStructureRatio(ChassisLocations location)
    {
        return location == ChassisLocations.Head ? 3 : 2;
    }
}