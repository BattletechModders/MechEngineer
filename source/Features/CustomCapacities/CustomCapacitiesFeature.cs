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
        var capacity = GetCarryCapacity(mechDef);
        var usage = GetCarryUsage(mechDef);
        if (PrecisionUtils.SmallerThan(capacity, usage))
        {
            errors.Add(MechValidationType.Overweight, string.Format(Settings.ErrorOverweight, capacity, usage));
        }
    }

    private static float GetCarryCapacity(MechDef mechDef)
    {
        var baseCapacity = mechDef.Inventory
            .Select(x => x.GetComponent<CarryCapacityBaseAddendByChassisFactorCustom>())
            .Where(x => x != null)
            .Select(x => x.Value)
            .Aggregate(0f, (previous, value) => previous + mechDef.Chassis.Tonnage * value);

        if (PrecisionUtils.Equals(baseCapacity, 0))
        {
            return 0;
        }

        var factor = mechDef.Inventory
            .Select(x => x.GetComponent<CarryCapacityFactorCustom>())
            .Where(x => x != null)
            .Select(x => x.Value)
            .Aggregate(1f, (previous, value) => previous * value);

        var addend = mechDef.Inventory
            .Select(x => x.GetComponent<CarryCapacityAddendCustom>())
            .Where(x => x != null)
            .Select(x => x.Value)
            .Aggregate(0f, (previous, value) => previous + value);

        return baseCapacity * factor + addend;
    }

    private static float GetCarryUsage(MechDef mechDef)
    {
        return mechDef.Inventory
            .Select(x => x.GetComponent<CarryUsageCustom>())
            .Where(x => x != null)
            .Select(x => x.Value)
            .Aggregate(0f, (previous, value) => previous + value);
    }
}
