#nullable enable
using System;
using BattleTech;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.ArmorMaximizer.Maximizer;

internal class ArmorLocationState : IComparable<ArmorLocationState>
{
    internal ArmorLocation Location { get; }
    private int Max { get; }
    private int Target { get; }
    internal int Assigned { get; set; }
    internal ChassisLocationState? LinkedChassisLocationState { get; }

    internal ArmorLocationState(
        ArmorLocation location,
        int max, // actual max to calculate IsFull
        int target, // used to calculate priority
        int assigned,
        ChassisLocationState? linkedChassisLocationState // used for torso armor locations that have shared limits
    ) {
        Location = location;
        Max = max;
        Target = target;
        Assigned = Mathf.Min(assigned, target);
        LinkedChassisLocationState = linkedChassisLocationState;
    }

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
    private float ArmorPointsPriority => Target / (float)Assigned;
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
        return $"[Location={Location} Max={Max} Target={Target} Assigned={Assigned} ChassisLocationState={LinkedChassisLocationState}]";
    }
}