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

    public void ValidateMech(MechDef mechDef, Errors errors)
    {
        ValidateCarryWeight(mechDef, errors);
    }

    // Carry Capacity - TacOps p.92
    // HandHeld Weapons - TacOps p.316
    private void ValidateCarryWeight(MechDef mechDef, Errors errors)
    {
        var totalCapacity = 0f;
        var totalUsage = 0f;

        var globalCapacityFactor = mechDef.Inventory
            .Select(x => x.GetComponent<CarryCapacityFactorCustom>())
            .Where(x => x != null)
            .Select(x => x.Value)
            .Aggregate(1f, (previous, value) => previous * value);

        MinHandReq CheckArm(ChassisLocations location)
        {
            var capacity = GetCarryCapacity(mechDef, location, globalCapacityFactor);
            var usage = GetCarryUsage(mechDef, location);

            totalCapacity += capacity;
            totalUsage += usage;

            if (PrecisionUtils.SmallerThan(capacity, usage))
            {
                return MinHandReq.Two;
            }
            if (PrecisionUtils.SmallerThan(0, usage))
            {
                return MinHandReq.One;
            }
            return MinHandReq.None;
        }
        var left = CheckArm(ChassisLocations.LeftArm);
        var right = CheckArm(ChassisLocations.RightArm);

        if (PrecisionUtils.SmallerThan(totalCapacity, totalUsage))
        {
            errors.Add(MechValidationType.Overweight, Settings.ErrorOverweight);
        }

        if ((left == MinHandReq.Two && right != MinHandReq.None) || (right == MinHandReq.Two && left != MinHandReq.None))
        {
            errors.Add(MechValidationType.Overweight, Settings.ErrorOneFreeHand);
        }
    }

    private enum MinHandReq
    {
        None,
        One,
        Two
    }

    private static float GetCarryCapacity(MechDef mechDef, ChassisLocations location, float globalCapacityFactor)
    {
        var baseCapacity = mechDef.Inventory
            .Where(x => x.MountedLocation == location)
            .Select(x => x.GetComponent<CarryCapacityOnArmChassisFactorCustom>())
            .Where(x => x != null)
            .Select(x => x.Value)
            .Aggregate(0f, (previous, value) => previous + mechDef.Chassis.Tonnage * value);

        if (PrecisionUtils.Equals(baseCapacity, 0))
        {
            return 0;
        }

        return baseCapacity * globalCapacityFactor;
    }

    private static float GetCarryUsage(MechDef mechDef, ChassisLocations location)
    {
        return mechDef.Inventory
            .Where(x => x.MountedLocation == location)
            .Select(x => x.GetComponent<CarryUsageCustom>())
            .Where(x => x != null)
            .Select(x => x.Value)
            .Aggregate(0f, (previous, value) => previous + value);
    }
}
