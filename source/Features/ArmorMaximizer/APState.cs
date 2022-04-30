using System.Collections.Generic;
using BattleTech;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.ArmorMaximizer;

internal class APState
{
    internal Dictionary<ChassisLocations, LocationState> Locations = new();
    internal class LocationState
    {
        internal int Assigned;
        internal int Max;
        public LocationState(float assigned, float max)
        {
            Max = PrecisionUtils.RoundDownToInt(max);
            Assigned = PrecisionUtils.RoundDownToInt(assigned);
            Assigned = Mathf.Min(Assigned, Max);
        }

        internal bool IsFull => Assigned >= Max;

        public override string ToString()
        {
            return $"[Assigned={Assigned} Max={Max}]";
        }
    }

    internal int Remaining;
    internal APState(MechDef mechDef)
    {
        Remaining = CalculateRemaining(mechDef);
        void Add(ChassisLocations location)
        {
            var locationDef = mechDef.Chassis.GetLocationDef(location);
            var loadoutDef = mechDef.GetLocationLoadoutDef(location);
            Locations[location] = new LocationState(
                loadoutDef.AssignedArmor + loadoutDef.AssignedRearArmor,
                ArmorStructureRatioFeature.GetMaximumArmorPoints(locationDef)
            );
        }
        foreach (var location in MechDefBuilder.Locations)
        {
            Add(location);
        }
    }

    private static int CalculateRemaining(MechDef mechDef)
    {
        var tonsPerPoint = ArmorUtils.TonPerPointWithFactor(mechDef);
        var maxPointsWithoutWeightLimit = (int)ArmorStructureRatioFeature.GetMaximumArmorPoints(mechDef);
        var maxWeightWithoutWeightLimit = maxPointsWithoutWeightLimit * tonsPerPoint;

        var weights = new Weights(mechDef)
        {
            StandardArmorWeight = 0
        };
        var maxWeight = Mathf.Min(maxWeightWithoutWeightLimit, weights.FreeWeight);
        var maxPoints = PrecisionUtils.RoundDownToInt(maxWeight / tonsPerPoint);

        var assigned = PrecisionUtils.RoundDownToInt(mechDef.MechDefAssignedArmor);
        Control.Logger.Trace?.Log($"CalculateRemaining tonsPerPoint={tonsPerPoint} maxWeightWithoutWeightLimit={maxWeightWithoutWeightLimit} maxWeight={maxWeight} maxPoints={maxPoints} assigned={assigned}");
        return maxPoints - assigned;
    }
}