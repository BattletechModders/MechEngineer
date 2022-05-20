using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
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

    internal static void CalculateCustomCapacityResults(
        MechDef mechDef,
        CustomCapacitiesSettings.CustomCapacity customCapacity,
        out BaseDescriptionDef description,
        out string text,
        out UIColor color,
        out bool show)
    {
        var id = customCapacity.Description.Id;

        float usage, capacity;
        bool hasError;
        if (customCapacity == Shared.Settings.CarryWeight)
        {
            var context = CalculateCarryWeight(mechDef);
            capacity = context.TotalCapacity;
            usage = context.TotalUsage;
            hasError = context.IsTotalOverweight || context.IsHandOverweight || context.IsHandMissingFreeHand;
            description = new(customCapacity.Description);
            description.Details +=
                $"\r\n" +
                $"\r\n<i>Total</i>   usage <b>{context.TotalUsage}</b>   capacity <b>{context.TotalCapacity}</b>" +
                $"\r\n<i>Mech</i>   usage <b>{context.MechUsage}</b>   capacity <b>{context.MechCapacity}</b>" +
                $"\r\n<i>HandHeld</i>   usage <b>{context.HandUsage}</b>   capacity <b>{context.HandCapacity}</b>" +
                $"\r\n<i>Hand Requirements</i>   left <b>{context.LeftHandReq}</b>   right <b>{context.RightHandReq}</b>";
        }
        else
        {
            CalculateCapacity(
                mechDef,
                ChassisLocations.All,
                id,
                out capacity,
                out usage
            );
            hasError = PrecisionUtils.SmallerThan(capacity, usage);
            description = customCapacity.Description;
        }
        color = hasError ? UIColor.Red : UIColor.White;
        text = string.Format(customCapacity.Format, usage, capacity);
        show = !(customCapacity.HideIfNoUsageAndCapacity && PrecisionUtils.Equals(capacity, 0) && PrecisionUtils.Equals(usage, 0));
    }

    public void ValidateMech(MechDef mechDef, Errors errors)
    {
        ValidateCarryWeight(mechDef, errors);
    }

    // Carry Capacity - TacOps p.92
    // HandHeld Weapons - TacOps p.316
    private void ValidateCarryWeight(MechDef mechDef, Errors errors)
    {
        var context = CalculateCarryWeight(mechDef);

        if (context.IsHandOverweight)
        {
            errors.Add(MechValidationType.Overweight, Settings.CarryHandErrorOverweight);
        }
        else if (context.IsHandMissingFreeHand)
        {
            errors.Add(MechValidationType.Overweight, Settings.CarryHandErrorOneFreeHand);
        }
        else if (context.IsTotalOverweight)
        {
            errors.Add(MechValidationType.Overweight, Settings.CarryWeight.ErrorOverweight);
        }
    }

    internal static CarryContext CalculateCarryWeight(MechDef mechDef)
    {
        var context = new CarryContext();
        CalculateCarryMech(mechDef, context);
        CalculateCarryHand(mechDef, context);
        return context;
    }

    private static void CalculateCarryMech(MechDef mechDef, CarryContext context)
    {
        CalculateCapacity(
            mechDef,
            ChassisLocations.All,
            CarryOnMechCollectionId,
            out context.MechCapacity,
            out context.MechUsage
        );
    }

    private static void CalculateCarryHand(MechDef mechDef, CarryContext context)
    {
        MinHandReq CheckArm(ChassisLocations location)
        {
            CalculateCapacity(mechDef, location, CarryInHandCollectionId, out var capacityOnLocation, out var usageOnLocation);

            context.HandCapacity += capacityOnLocation;
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

    internal class CarryContext
    {
        internal float HandCapacity;
        internal float HandUsage;

        internal bool IsHandOverweight => PrecisionUtils.SmallerThan(HandCapacity, HandUsage);

        internal float MechCapacity;
        internal float MechUsage;

        internal float TotalCapacity => HandCapacity + MechCapacity;
        internal float TotalUsage => HandUsage + MechUsage;

        internal bool IsTotalOverweight => PrecisionUtils.SmallerThan(TotalCapacity, TotalUsage);

        internal MinHandReq LeftHandReq;
        internal MinHandReq RightHandReq;

        internal bool IsHandMissingFreeHand =>
            (LeftHandReq == MinHandReq.Two && RightHandReq != MinHandReq.None)
            || (RightHandReq == MinHandReq.Two && LeftHandReq != MinHandReq.None);
    }

    internal enum MinHandReq
    {
        None,
        One,
        Two
    }

    internal const string CarryInHandCollectionId = "CarryInHand";
    internal const string CarryOnMechCollectionId = "CarryOnMech";

    private static void CalculateCapacity(MechDef mechDef, ChassisLocations location, string collectionId, out float capacity, out float usage)
    {
        var mods = mechDef.Inventory
            .SelectMany(r =>
                r.GetComponents<CapacityModCustom>()
                .Where(mod => mod.Collection == collectionId)
                .Where(mod => !mod.IsLocationRestricted || (r.MountedLocation & location) != ChassisLocations.None)
            )
            .OrderBy(m => m.Priority)
            .ThenBy(m => m.Operation)
            .ToList();

        float ApplyOperation(float previous, CapacityModCustom mod)
        {
            var factor = mod.QuantityFactorType switch
            {
                QuantityFactorType.One => 1f,
                QuantityFactorType.ChassisTonnage => mechDef.Chassis.Tonnage,
                _ => throw new ArgumentOutOfRangeException()
            };
            var value = mod.Quantity * factor;
            return mod.Operation switch
            {
                OperationType.Set => value,
                OperationType.Add => previous + value,
                OperationType.Multiply => previous * value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        capacity = mods
            .Where(m => !m.IsUsage)
            .Aggregate(0f, ApplyOperation);

        usage = mods
            .Where(m => m.IsUsage)
            .Aggregate(0f, ApplyOperation);
    }
}
