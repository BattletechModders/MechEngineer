using System;
using BattleTech;

namespace MechEngineer.Features.DynamicSlots;

internal readonly struct LocationUsages
{
    public LocationUsages()
    {
    }

    internal int Length => _array.Length;
    internal ref LocationUsage this[int index] => ref _array[index];
    internal ref LocationUsage this[ChassisLocations location] => ref this[ToIndexed(location)];

    private readonly LocationUsage[] _array = {
        new(ChassisLocations.Head),
        new(ChassisLocations.LeftArm),
        new(ChassisLocations.LeftTorso),
        new(ChassisLocations.CenterTorso),
        new(ChassisLocations.RightTorso),
        new(ChassisLocations.RightArm),
        new(ChassisLocations.LeftLeg),
        new(ChassisLocations.RightLeg),
    };
    private static int ToIndexed(ChassisLocations location)
    {
        return location switch
        {
            ChassisLocations.Head => 0,
            ChassisLocations.LeftArm => 1,
            ChassisLocations.LeftTorso => 2,
            ChassisLocations.CenterTorso => 3,
            ChassisLocations.RightTorso => 4,
            ChassisLocations.RightArm => 5,
            ChassisLocations.LeftLeg => 6,
            ChassisLocations.RightLeg => 7,
            _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
        };
    }

    internal struct LocationUsage
    {
        internal LocationUsage(ChassisLocations location)
        {
            Location = location;
        }
        internal readonly ChassisLocations Location;

        // 1. slots and inventory pass
        internal int Max; // from chassis
        internal int Inventory; // from inventory

        // 2. dynamics pass
        internal int DynamicLocationalLocal; // usage that fits on location or could not be transferred
        internal int DynamicLocationalTransferred; // usage coming from an adjacent location, can't be further transferred

        // set by RefreshSummary after 2. pass
        internal int Usage;
        internal int Free;
        internal int Missing;
        internal int FreeIncludingDynamic;

        internal void RefreshSummary()
        {
            Usage = Inventory + DynamicLocationalLocal + DynamicLocationalTransferred;
            Free = Math.Max(Max - Usage, 0);
            Missing = Math.Max(Usage - Max, 0);
            FreeIncludingDynamic = Math.Max(Max - Inventory, 0);
        }

        // 3. fixed slots pass
        internal int Fixed;
        internal int FreeIncludingDynamicMovable;
        internal void SetFixed(int adjacentFreeSlots, int globalFreeSlots)
        {
            var minUsageDueToGlobal = Math.Max(Max - globalFreeSlots, 0);
            var nonTransferableDynamicLocational = Math.Max(DynamicLocationalLocal - adjacentFreeSlots, 0);
            var minFixedUsageMinusTransferable = Math.Min(
                Max,
                Inventory + DynamicLocationalTransferred + nonTransferableDynamicLocational
            );
            Fixed = Math.Max(minUsageDueToGlobal, minFixedUsageMinusTransferable);
            FreeIncludingDynamicMovable = Math.Max(Max - Fixed, 0);
        }

        public override string ToString()
        {
            return $"{nameof(Location)}={Location}" +
                   $" {nameof(Max)}={Max}" +
                   $" {nameof(Inventory)}={Inventory}" +
                   $" {nameof(DynamicLocationalLocal)}={DynamicLocationalLocal}" +
                   $" {nameof(DynamicLocationalTransferred)}={DynamicLocationalTransferred}" +
                   $" {nameof(Usage)}={Usage}" +
                   $" {nameof(Free)}={Free}" +
                   $" {nameof(Missing)}={Missing}" +
                   $" {nameof(FreeIncludingDynamic)}={FreeIncludingDynamic}" +
                   $" {nameof(Fixed)}={Fixed}" +
                   $" {nameof(FreeIncludingDynamicMovable)}={FreeIncludingDynamicMovable}";
        }
    }
}