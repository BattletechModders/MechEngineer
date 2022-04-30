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
        internal float Assigned;
        internal float Max;
        public LocationState(float assigned, float max)
        {
            Assigned = assigned;
            Max = max;
        }

        internal bool IsFull => Assigned < Max;

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
        var tonsPerPoint = mechDef.TonPerPointWithFactor();
        var maxPointsWithoutWeightLimit = (int)ArmorStructureRatioFeature.GetMaximumArmorPoints(mechDef);
        var maxWeightWithoutWeightLimit = maxPointsWithoutWeightLimit * tonsPerPoint;

        var weights = new Weights(mechDef)
        {
            StandardArmorWeight = 0
        };
        var maxWeight = Mathf.Min(maxWeightWithoutWeightLimit, weights.FreeWeight);
        var maxPoints = PrecisionUtils.RoundDownToInt(maxWeight / tonsPerPoint);

        var assigned = PrecisionUtils.RoundDownToInt(mechDef.MechDefAssignedArmor);
        return maxPoints - assigned;
    }
}