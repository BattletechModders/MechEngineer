using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.ArmorMaximizer.Maximizer;

internal class MechArmorState
{
    internal static bool Maximize(MechDef mechDef, bool ignoreLocks, int armorPerStep, out List<ArmorLocationState> updates)
    {
        var mechArmorState = new MechArmorState(mechDef, ignoreLocks);
        return mechArmorState.Maximize(armorPerStep, out updates);
    }

    internal static bool Strip(MechDef mechDef, bool ignoreLocks, out List<ArmorLocationState> updates)
    {
        var mechArmorState = new MechArmorState(mechDef, ignoreLocks);
        return mechArmorState.Strip(out updates);
    }

    private List<ArmorLocationState> Locations { get; } = new();

    private bool LocksEnabled { get; }
    private int Max { get; }
    private int Assigned { get; set; }
    private bool HasChanges { get; set; }

    private int Remaining => Max - Assigned;

    private MechArmorState(MechDef mechDef, bool ignoreLocks)
    {
        LocksEnabled = !ignoreLocks;
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
        var maxPointsWithoutWeightLimit = ArmorUtils.GetMaximumArmorPoints(mechDef);
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
        var armorLocationState = new ArmorLocationState(
            armorLocation,
            GetArmorAllocationPriority(armorLocation),
            PrecisionUtils.RoundDownToInt(locationDef.MaxArmor),
            PrecisionUtils.RoundDownToInt(loadoutDef.AssignedArmor),
            null
        );
        Locations.Add(armorLocationState);
    }

    private void PrepareTorsoLocation(ChassisLocations chassisLocation, LocationDef locationDef, LocationLoadoutDef loadoutDef)
    {
        var chassisLocationState = new ChassisLocationState(
            ArmorUtils.GetMaximumArmorPoints(locationDef),
            PrecisionUtils.RoundDownToInt(loadoutDef.AssignedArmor + loadoutDef.AssignedRearArmor)
        );

        {
            var armorLocation = chassisLocation.ToArmorLocation(false);
            var armorLocationState = new ArmorLocationState(
                armorLocation,
                GetArmorAllocationPriority(armorLocation),
                PrecisionUtils.RoundDownToInt(locationDef.MaxArmor),
                PrecisionUtils.RoundDownToInt(loadoutDef.AssignedArmor),
                chassisLocationState
            );
            Locations.Add(armorLocationState);
        }

        {
            var armorLocation = chassisLocation.ToArmorLocation(true);
            var armorLocationState = new ArmorLocationState(
                armorLocation,
                GetArmorAllocationPriority(armorLocation),
                PrecisionUtils.RoundDownToInt(locationDef.MaxRearArmor),
                PrecisionUtils.RoundDownToInt(loadoutDef.AssignedRearArmor),
                chassisLocationState
            );
            Locations.Add(armorLocationState);
        }
    }

    private static int GetArmorAllocationPriority(ArmorLocation location)
    {
        var constants = UnityGameInstance.BattleTechGame.MechStatisticsConstants;
        var ratio = location switch
        {
            ArmorLocation.Head => constants.ArmorAllocationRatioHead,
            ArmorLocation.CenterTorso => constants.ArmorAllocationRatioCenterTorso,
            ArmorLocation.CenterTorsoRear => constants.ArmorAllocationRatioCenterTorsoRear,
            ArmorLocation.LeftTorso => constants.ArmorAllocationRatioLeftTorso,
            ArmorLocation.LeftTorsoRear => constants.ArmorAllocationRatioLeftTorsoRear,
            ArmorLocation.RightTorso => constants.ArmorAllocationRatioRightTorso,
            ArmorLocation.RightTorsoRear => constants.ArmorAllocationRatioRightTorsoRear,
            ArmorLocation.LeftArm => constants.ArmorAllocationRatioLeftArm,
            ArmorLocation.RightArm => constants.ArmorAllocationRatioRightArm,
            ArmorLocation.LeftLeg => constants.ArmorAllocationRatioLeftLeg,
            ArmorLocation.RightLeg => constants.ArmorAllocationRatioRightLeg,
            _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
        };
        const int someMultiplierLargeEnoughSoDividingByAssignedWorksAsPriority = 1000000;
        return PrecisionUtils.RoundDownToInt(ratio * someMultiplierLargeEnoughSoDividingByAssignedWorksAsPriority);
    }

    private bool Strip(out List<ArmorLocationState> updates)
    {
        updates = Locations.ToList();
        updates.RemoveAll(s => s.IsEmpty);
        if (LocksEnabled)
        {
            updates.RemoveAll(s => ArmorLocationLocker.IsLocked(s.Location));
        }
        foreach (var location in updates)
        {
            if (location.LinkedChassisLocationState != null)
            {
                location.LinkedChassisLocationState.Assigned -= location.Assigned;
            }
            Assigned -= location.Assigned;
            location.Assigned = 0;
        }
        return updates.Count > 0;
    }

    private bool Maximize(int armorPerStep, out List<ArmorLocationState> updates)
    {
        if (ArmorMaximizerFeature.Shared.Settings.StripBeforeMax)
        {
            Strip(out _);
        }

        updates = Locations.ToList();
        updates.RemoveAll(s => s.IsFull || (s.LinkedChassisLocationState?.IsFull ?? false));
        if (LocksEnabled)
        {
            updates.RemoveAll(s => ArmorLocationLocker.IsLocked(s.Location));
        }
        var tmp = updates.ToList();

        // this does a lot of looping, just 20ms on my machine though for an empty atlas
        Control.Logger.Trace?.Log($"Maximize before loop");
        while (Remaining > 0)
        {
            tmp.RemoveAll(s => s.IsFull || (s.LinkedChassisLocationState?.IsFull ?? false));
            if (tmp.Count == 0)
            {
                break;
            }
            Increment(tmp.Max(), armorPerStep);
        }
        Control.Logger.Trace?.Log($"Maximize after loop");

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

        Control.Logger.Trace?.Log($"Increment change={change} Assigned={Assigned} armorLocationState={armorLocationState}");
        HasChanges = true;
    }
}