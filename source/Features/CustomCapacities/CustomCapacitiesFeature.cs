using System.Linq;
using BattleTech;
using CustomComponents;
using MechEngineer.Features.OverrideTonnage;
using MechEngineer.Helper;
using MechEngineer.Misc;

namespace MechEngineer.Features.CustomCapacities;

internal class CustomCapacitiesFeature : Feature<CustomCapacitiesSettings>, IValidateMech
{
    internal static readonly CustomCapacitiesFeature Shared = new();

    internal override CustomCapacitiesSettings Settings => Control.Settings.CustomCapacities;

    protected override void SetupFeatureLoaded()
    {
        var ccValidation = new CCValidationAdapter(this);
        Validator.RegisterMechValidator(ccValidation.ValidateMech, ccValidation.ValidateMechCanBeFielded);
    }

    internal static void CalculateCarryWeightResults(MechDef mechDef, out float capacity, out float usage)
    {
        var context = CalculateCarryWeight(mechDef);
        capacity = context.TotalCapacity;
        usage = context.TotalUsage;
    }

    public void ValidateMech(MechDef mechDef, Errors errors)
    {
        ValidateCarryWeight(mechDef, errors);
    }

    // Carry Capacity - TacOps p.92
    // HandHeld Weapons - TacOps p.316
    private void ValidateCarryWeight(MechDef mechDef, Errors errors)
    {
        var result = CalculateCarryWeight(mechDef);

        if (PrecisionUtils.SmallerThan(result.HandCapacity, result.HandUsage))
        {
            errors.Add(MechValidationType.Overweight, Settings.CarryHandErrorOverweight);
        }
        else if ((result.LeftHandReq == MinHandReq.Two && result.RightHandReq != MinHandReq.None) || (result.RightHandReq == MinHandReq.Two && result.LeftHandReq != MinHandReq.None))
        {
            errors.Add(MechValidationType.Overweight, Settings.CarryHandErrorOneFreeHand);
        }
        else if (PrecisionUtils.SmallerThan(result.TotalCapacity, result.TotalUsage))
        {
            errors.Add(MechValidationType.Overweight, Settings.CarryTotalErrorOverweight);
        }
    }

    private static CarryContext CalculateCarryWeight(MechDef mechDef)
    {
        var result = new CarryContext
        {
            CapacityFactor = GetCapacityFactor(mechDef)
        };
        CalculateCarryHand(mechDef, result);
        CalculateCarryMech(mechDef, result);
        return result;
    }

    private static void CalculateCarryMech(MechDef mechDef, CarryContext context)
    {
        context.MechCapacityRaw = GetCarryMechCapacity(mechDef);
        context.MechUsage = GetCarryMechUsage(mechDef);
    }

    private static void CalculateCarryHand(MechDef mechDef, CarryContext context)
    {
        MinHandReq CheckArm(ChassisLocations location)
        {
            var capacityOnLocation = GetCarryHandCapacityOnLocation(mechDef, location);
            var usageOnLocation = GetCarryHandUsageOnLocation(mechDef, location);

            context.HandCapacityRaw += capacityOnLocation;
            context.HandUsage += usageOnLocation;

            if (PrecisionUtils.SmallerThan(capacityOnLocation, usageOnLocation))
            {
                return MinHandReq.Two;
            }
            if (PrecisionUtils.SmallerThan(0, usageOnLocation))
            {
                return MinHandReq.One;
            }
            return MinHandReq.None;
        }
        context.LeftHandReq = CheckArm(ChassisLocations.LeftArm);
        context.RightHandReq = CheckArm(ChassisLocations.RightArm);
    }

    private class CarryContext
    {
        internal float CapacityFactor;

        internal float HandCapacityRaw;
        internal float HandCapacity => HandCapacityRaw * CapacityFactor;
        internal float HandUsage;

        internal float MechCapacityRaw;
        private float MechCapacity => MechCapacityRaw * CapacityFactor;
        internal float MechUsage;

        internal float TotalCapacity => HandCapacity + MechCapacity;
        internal float TotalUsage => HandUsage + MechUsage;

        internal MinHandReq LeftHandReq;
        internal MinHandReq RightHandReq;
    }

    private enum MinHandReq
    {
        None,
        One,
        Two
    }

    private static float GetCapacityFactor(MechDef mechDef)
    {
        return mechDef.Inventory
            .Select(x => x.GetComponent<CarryCapacityFactorCustom>())
            .Where(x => x != null)
            .Select(x => x.Value)
            .Aggregate(1f, (previous, value) => previous * value);
    }

    private static float GetCarryHandCapacityOnLocation(MechDef mechDef, ChassisLocations location)
    {
        return mechDef.Inventory
            .Where(x => x.MountedLocation == location)
            .Select(x => x.GetComponent<CarryHandCapacityChassisFactorCustom>())
            .Where(x => x != null)
            .Select(x => x.Value)
            .Aggregate(0f, (previous, value) => previous + mechDef.Chassis.Tonnage * value);
    }

    private static float GetCarryHandUsageOnLocation(MechDef mechDef, ChassisLocations location)
    {
        return mechDef.Inventory
            .Where(x => x.MountedLocation == location)
            .Select(x => x.GetComponent<CarryHandUsageCustom>())
            .Where(x => x != null)
            .Select(x => x.Value)
            .Aggregate(0f, (previous, value) => previous + value);
    }

    private static float GetCarryMechCapacity(MechDef mechDef)
    {
        return mechDef.Inventory
            .Select(x => x.GetComponent<CarryMechCapacityChassisFactorCustom>())
            .Where(x => x != null)
            .Select(x => x.Value)
            .Aggregate(0f, (previous, value) => previous + mechDef.Chassis.Tonnage * value);
    }

    private static float GetCarryMechUsage(MechDef mechDef)
    {
        return mechDef.Inventory
            .Select(x => x.GetComponent<CarryMechUsageCustom>())
            .Where(x => x != null)
            .Select(x => x.Value)
            .Aggregate(0f, (previous, value) => previous + value);
    }
}
