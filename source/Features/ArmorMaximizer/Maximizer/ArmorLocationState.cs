#nullable enable
using System;
using BattleTech;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.ArmorMaximizer.Maximizer;

internal class ArmorLocationState : IComparable<ArmorLocationState>
{
    internal ArmorLocation Location { get; }
    private float BasePriority { get; }
    private int Max { get; }
    internal int Assigned { get; set; }
    internal ChassisLocationState? LinkedChassisLocationState { get; }

    internal ArmorLocationState(
        ArmorLocation location,
        float basePriority, // used to calculate priority, priority is reduced linearly by assigned points
        int max, // actual max to calculate IsFull
        int assigned,
        ChassisLocationState? linkedChassisLocationState
        // used for torso armor locations that have shared limits
        ) {
        Location = location;
        BasePriority = basePriority;
        Max = max;
        Assigned = assigned;
        LinkedChassisLocationState = linkedChassisLocationState;
    }

    internal bool IsEmpty => Assigned <= 0;
    internal bool IsFull => Assigned >= Max;
    internal int Remaining => Max - Assigned;

    public int CompareTo(ArmorLocationState other)
    {
        var cmp = ArmorPointsPriority - other.ArmorPointsPriority;
        if (!PrecisionUtils.Equals(cmp, 0))
        {
            return cmp > 0 ? 1 : -1;
        }
        return LocationPriority - other.LocationPriority;
    }
    private float ArmorPointsPriority => BasePriority / Assigned;
    private int LocationPriority => Location switch
    {
        ArmorLocation.Head => 60,
        ArmorLocation.CenterTorso => 51,
        ArmorLocation.CenterTorsoRear => 50,
        ArmorLocation.LeftTorso => 41,
        ArmorLocation.RightTorso => 40,
        ArmorLocation.LeftTorsoRear => 31,
        ArmorLocation.RightTorsoRear => 30,
        ArmorLocation.LeftLeg => 21,
        ArmorLocation.RightLeg => 20,
        ArmorLocation.LeftArm => 11,
        ArmorLocation.RightArm => 10,
        _ => 0
    };

    public override string ToString()
    {
        return $"[Location={Location} BasePriority={BasePriority} Max={Max} Assigned={Assigned} ChassisLocationState={LinkedChassisLocationState}]";
    }
}