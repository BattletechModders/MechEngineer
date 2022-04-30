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
        internal ChassisLocations Location;
        internal int Max;
        internal int Assigned;

        internal LocationState(ChassisLocations location, int max, int assigned)
        {
            Location = location;
            Max = max;
            Assigned = assigned;
            Assigned = Mathf.Min(Assigned, Max);
        }

        internal bool IsFull => Missing <= 0;
        private int Missing => Max - Assigned;

        internal int PriorityPrimary => Missing;
        internal int PrioritySecondary => Location switch
        {
            ChassisLocations.Head => 10,
            ChassisLocations.CenterTorso => 9,
            ChassisLocations.LeftTorso => 8,
            ChassisLocations.RightTorso => 7,
            ChassisLocations.LeftLeg => 6,
            ChassisLocations.RightLeg => 5,
            ChassisLocations.LeftArm => 4,
            ChassisLocations.RightArm => 3,
            _ => 0
        };

        public override string ToString()
        {
            return $"[Location={Location} Assigned={Assigned} Max={Max}]";
        }
    }

    internal int Remaining;
    internal APState(MechDef mechDef)
    {
        var mechMax = CalculateMaximum(mechDef);
        var mechAssigned = PrecisionUtils.RoundDownToInt(mechDef.MechDefAssignedArmor);
        Remaining = mechMax - mechAssigned;
        void Add(ChassisLocations location)
        {
            var locationDef = mechDef.Chassis.GetLocationDef(location);
            var loadoutDef = mechDef.GetLocationLoadoutDef(location);
            var locationMax = ArmorStructureRatioFeature.GetMaximumArmorPoints(locationDef);
            var locationAssigned = PrecisionUtils.RoundDownToInt(loadoutDef.AssignedArmor + loadoutDef.AssignedRearArmor);
            Locations[location] = new LocationState(
                location,
                locationMax,
                locationAssigned
            );
        }
        foreach (var location in MechDefBuilder.Locations)
        {
            Add(location);
        }
    }

    private static int CalculateMaximum(MechDef mechDef)
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
        return maxPoints;
    }
}