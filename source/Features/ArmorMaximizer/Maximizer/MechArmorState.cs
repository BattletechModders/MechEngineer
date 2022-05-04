#nullable enable
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.ArmorMaximizer.Maximizer;

internal class MechArmorState
{
    internal static bool Maximize(MechDef mechDef, int armorPerStep, out List<ArmorLocationState> updates)
    {
        var mechArmorState = new MechArmorState(mechDef);
        return mechArmorState.Maximize(armorPerStep, out updates);
    }

    internal static bool Strip(MechDef mechDef, out List<ArmorLocationState> updates)
    {
        var mechArmorState = new MechArmorState(mechDef);
        return mechArmorState.Strip(out updates);
    }

    private List<ArmorLocationState> Locations { get; } = new();

    private int Max { get; }
    private int Assigned { get; set; }
    private bool HasChanges { get; set; }

    private int Remaining => Max - Assigned;

    private MechArmorState(MechDef mechDef)
    {
        Max = CalculateMaximum(mechDef);
        Assigned = PrecisionUtils.RoundDownToInt(mechDef.MechDefAssignedArmor);

        foreach (var chassisLocation in LocationExtensions.ChassisLocationList)
        {
            var locationDef = mechDef.Chassis.GetLocationDef(chassisLocation);
            var loadoutDef = mechDef.GetLocationLoadoutDef(chassisLocation);
            if ((chassisLocation & ChassisLocations.Torso) == ChassisLocations.None)
            {
                PrepareNonTorsoLocation(chassisLocation, locationDef, loadoutDef);
            }
            else
            {
                PrepareTorsoLocation(chassisLocation, locationDef, loadoutDef);
            }
        }
    }

    private static int CalculateMaximum(MechDef mechDef)
    {
        var tonsPerPoint = ArmorUtils.TonPerPointWithFactor(mechDef);
        var maxPointsWithoutWeightLimit = ArmorStructureRatioFeature.GetMaximumArmorPoints(mechDef);
        var maxWeightWithoutWeightLimit = maxPointsWithoutWeightLimit * tonsPerPoint;

        var weights = new Weights(mechDef)
        {
            StandardArmorWeight = 0
        };
        var maxWeight = Mathf.Min(maxWeightWithoutWeightLimit, weights.FreeWeight);
        var maxPoints = PrecisionUtils.RoundDownToInt(maxWeight / tonsPerPoint);
        return maxPoints;
    }

    private void PrepareNonTorsoLocation(ChassisLocations chassisLocation, LocationDef locationDef, LocationLoadoutDef loadoutDef)
    {
        var armorLocation = chassisLocation.ToArmorLocation(false);
        var locationMax = PrecisionUtils.RoundDownToInt(locationDef.MaxArmor);
        var locationAssigned = PrecisionUtils.RoundDownToInt(loadoutDef.AssignedArmor);
        var armorLocationState = new ArmorLocationState(
            armorLocation,
            locationMax,
            locationMax,
            locationAssigned,
            null
        );
        Locations.Add(armorLocationState);
    }

    private void PrepareTorsoLocation(ChassisLocations chassisLocation, LocationDef locationDef, LocationLoadoutDef loadoutDef)
    {
        var chassisLocationState = new ChassisLocationState(
            ArmorStructureRatioFeature.GetMaximumArmorPoints(locationDef),
            PrecisionUtils.RoundDownToInt(loadoutDef.AssignedArmor + loadoutDef.AssignedRearArmor)
        );

        var ratio = ArmorMaximizerFeature.Shared.Settings.TorsoFrontBackRatio;
        var locationTargetFront = PrecisionUtils.RoundDownToInt(chassisLocationState.Max * ratio);
        var locationTargetBack = chassisLocationState.Max - locationTargetFront;

        {
            var armorLocation = chassisLocation.ToArmorLocation(false);
            var locationMax = PrecisionUtils.RoundDownToInt(locationDef.MaxArmor);
            var locationAssigned = PrecisionUtils.RoundDownToInt(loadoutDef.AssignedArmor);
            var armorLocationState = new ArmorLocationState(
                armorLocation,
                locationMax,
                locationTargetFront,
                locationAssigned,
                chassisLocationState
            );
            Locations.Add(armorLocationState);
        }

        {
            var armorLocation = chassisLocation.ToArmorLocation(true);
            var locationMax = PrecisionUtils.RoundDownToInt(locationDef.MaxRearArmor);
            var locationAssigned = PrecisionUtils.RoundDownToInt(loadoutDef.AssignedRearArmor);
            var armorLocationState = new ArmorLocationState(
                armorLocation,
                locationMax,
                locationTargetBack,
                locationAssigned,
                chassisLocationState
            );
            Locations.Add(armorLocationState);
        }
    }

    private bool Strip(out List<ArmorLocationState> updates)
    {
        Locations.RemoveAll(s => s.IsEmpty || ArmorLocationLocker.IsLocked(s.Location));
        updates = Locations.ToList();
        foreach (var location in Locations)
        {
            if (location.LinkedChassisLocationState != null)
            {
                location.LinkedChassisLocationState.Assigned -= location.Assigned;
            }
            location.Assigned = 0;
        }
        return updates.Count > 0;
    }

    private bool Maximize(int armorPerStep, out List<ArmorLocationState> updates)
    {
        Locations.RemoveAll(s => s.IsFull || (s.LinkedChassisLocationState?.IsFull ?? false) || ArmorLocationLocker.IsLocked(s.Location));
        updates = Locations.ToList();

        while (Remaining > 0)
        {
            Locations.RemoveAll(s => s.IsFull || (s.LinkedChassisLocationState?.IsFull ?? false));
            if (Locations.Count == 0)
            {
                break;
            }
            Increment(Locations.Max(), armorPerStep);
        }
        return HasChanges;
    }

    private void Increment(ArmorLocationState armorLocationState, int step)
    {
        var change = step;

        change = Mathf.Min(change, Remaining);
        change = Mathf.Min(change, armorLocationState.Remaining);
        if (armorLocationState.LinkedChassisLocationState != null)
        {
            change = Mathf.Min(change, armorLocationState.LinkedChassisLocationState.Remaining);
        }

        if (change <= 0)
        {
            return;
        }

        Assigned += change;
        armorLocationState.Assigned += change;
        if (armorLocationState.LinkedChassisLocationState != null)
        {
            armorLocationState.LinkedChassisLocationState.Assigned += change;
        }

        Control.Logger.Trace?.Log($"Increment Location={armorLocationState.Location} change={change} Assigned={Assigned} armor.Assigned={armorLocationState.Assigned} chassis.Assigned={armorLocationState.LinkedChassisLocationState?.Assigned}");
        HasChanges = true;
    }
}