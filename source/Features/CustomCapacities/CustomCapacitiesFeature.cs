using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using MechEngineer.Features.OverrideTonnage;
using MechEngineer.Helper;
using MechEngineer.Misc;
using UnityEngine;

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

    internal void CalculateCustomCapacityResults(
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
            hasError = context.IsMechOverweight || context.IsHandOverweight || context.IsHandMissingFreeHand;
            description = new(customCapacity.Description);
            string ReqString(MinHandReq req)
            {
                return req switch
                {
                    MinHandReq.None => "",
                    MinHandReq.One => "one-handed",
                    MinHandReq.Two => "two-handed req.",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            description.Details +=
                $"\r\n" +
                (context.HasSharedTopOffCapacity ? $"\r\n<i>Shared</i>   usage <b>{context.SharedTopOffUsage:0.##} / {context.SharedTopOffCapacity:0.###}</b>" : "") +
                (context.HasMechUsageOrCapacity ? $"\r\n<i>Mech</i>   usage <b>{context.MechUsage:0.##} / {context.MechCapacity:0.###}</b>" : "") +
                $"\r\n<i>HandHeld</i>" +
                $"\r\n   <i>Total</i>   usage <b>{context.HandTotalUsage:0.###} / {context.HandTotalCapacity:0.###}</b>" +
                $"\r\n   <i>Left</i>    usage <b>{context.HandLeftUsage:0.###} / {context.HandLeftCapacity:0.###}</b>   <b>{ReqString(context.LeftHandReq)}</b>" +
                $"\r\n   <i>Right</i>   usage <b>{context.HandRightUsage:0.###} / {context.HandRightCapacity:0.###}</b>   <b>{ReqString(context.RightHandReq)}</b>";
        }
        else
        {
            CalculateCapacity(
                mechDef,
                id,
                ChassisLocations.All,
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
        else if (context.IsMechOverweight)
        {
            errors.Add(MechValidationType.Overweight, Settings.CarryWeight.ErrorOverweight);
        }
    }

    private CarryContext CalculateCarryWeight(MechDef mechDef)
    {
        var context = new CarryContext();
        CalculateSharedTopOff(mechDef, context);
        CalculateCarryMech(mechDef, context);
        CalculateCarryHand(mechDef, context);
        return context;
    }

    private void CalculateSharedTopOff(MechDef mechDef, CarryContext context)
    {
        bool HasHandActuator(ChassisLocations location)
        {
            return mechDef.Inventory.Any(x => x.MountedLocation == location && x.GetCategory(Settings.CarrySharedTopOffHandCategoryID) != null);
        }
        context.HasLeftHandActuator = HasHandActuator(ChassisLocations.LeftArm);
        context.HasRightHandActuator = HasHandActuator(ChassisLocations.RightArm);

        CalculateCapacity(
            mechDef,
            CarrySharedTopOffCollectionId,
            ChassisLocations.All,
            out context.SharedTopOffCapacity,
            out _,
            Settings.CarrySharedTopOff * mechDef.Chassis.Tonnage);
    }

    private static void CalculateCarryMech(MechDef mechDef, CarryContext context)
    {
        CalculateCapacity(
            mechDef,
            CarryOnMechCollectionId,
            ChassisLocations.All,
            out context.StatMechCapacity,
            out context.StatMechUsage);
    }

    private static void CalculateCarryHand(MechDef mechDef, CarryContext context)
    {
        CalculateCapacity(
            mechDef,
            CarryInHandCollectionId,
            ChassisLocations.LeftArm,
            out context.StatHandLeftCapacity,
            out context.StatHandLeftUsage
        );

        CalculateCapacity(
            mechDef,
            CarryInHandCollectionId,
            ChassisLocations.RightArm,
            out context.StatHandRightCapacity,
            out context.StatHandRightUsage
        );
    }

    private class CarryContext
    {
        internal bool IsHandOverweight => PrecisionUtils.SmallerThan(HandTotalCapacity, HandTotalUsage);
        internal bool IsMechOverweight => PrecisionUtils.SmallerThan(MechCapacity, MechUsage);

        internal bool IsHandMissingFreeHand =>
            (LeftHandReq == MinHandReq.Two && RightHandReq != MinHandReq.None)
            || (RightHandReq == MinHandReq.Two && LeftHandReq != MinHandReq.None);

        internal MinHandReq LeftHandReq => CalcHandReq(HandLeftCapacity, HandLeftUsage);
        internal MinHandReq RightHandReq => CalcHandReq(HandRightCapacity, HandRightUsage);

        private static MinHandReq CalcHandReq(float capacityOnLocation, float usageOnLocation)
        {
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

        internal float TotalCapacity => MechCapacity + HandTotalCapacity + LeftOverSharedTopOffCapacity;
        internal float TotalUsage => MechUsage + HandTotalUsage;

        internal float HandTotalCapacity => HandLeftCapacity + HandRightCapacity;
        internal float HandTotalUsage => HandLeftUsage + HandRightUsage;

        internal float HandLeftCapacity => TopOffHandLeftCapacity + StatHandLeftCapacity;
        internal float HandLeftUsage => StatHandLeftUsage;

        internal float HandRightCapacity => TopOffHandRightCapacity + StatHandRightCapacity;
        internal float HandRightUsage => StatHandRightUsage;

        internal bool HasMechUsageOrCapacity => !PrecisionUtils.Equals(0, MechCapacity) || !PrecisionUtils.Equals(0, MechUsage);

        internal float MechCapacity => TopOffMechCapacity + StatMechCapacity;
        internal float MechUsage => StatMechUsage;

        #region TopOff

        internal float SharedTopOffUsage => TopOffHandLeftCapacity + TopOffHandRightCapacity + TopOffMechCapacity;

        private float TopOffHandLeftCapacity => HasLeftHandActuator ? TopOffHandLeftCapacityIfHand : 0;
        private float TopOffHandLeftCapacityIfHand => Mathf.Min(TopOffHandLeftCapacityPotential, HandLeftCapacityMissing);
        private float TopOffHandLeftCapacityPotential => SharedTopOffCapacity / 2;
        private float HandLeftCapacityMissing => Mathf.Max(0, StatHandLeftUsage - StatHandLeftCapacity);

        private float TopOffHandRightCapacity => HasRightHandActuator ? TopOffHandRightCapacityIfHand : 0;
        private float TopOffHandRightCapacityIfHand => Mathf.Min(TopOffHandRightCapacityPotential, HandRightCapacityMissing);
        private float TopOffHandRightCapacityPotential => Mathf.Min(SharedTopOffCapacity / 2, SharedMinCapacityAfterLeftHand);
        private float HandRightCapacityMissing => Mathf.Max(0, StatHandRightUsage - StatHandRightCapacity);
        private float SharedMinCapacityAfterLeftHand => SharedTopOffCapacity - TopOffHandLeftCapacity;

        private float TopOffMechCapacity => Mathf.Min(SharedMinCapacityAfterRightHand, MechCapacityMissing);
        private float MechCapacityMissing => Mathf.Max(0, StatMechUsage - StatMechCapacity);
        private float SharedMinCapacityAfterRightHand => SharedMinCapacityAfterLeftHand - TopOffHandRightCapacity;

        private float LeftOverSharedTopOffCapacity => SharedMinCapacityAfterRightHand - TopOffMechCapacity;
        internal bool HasSharedTopOffCapacity => !PrecisionUtils.Equals(0, SharedTopOffCapacity);

        internal float SharedTopOffCapacity;
        internal bool HasLeftHandActuator;
        internal bool HasRightHandActuator;

        #endregion

        internal float StatHandLeftCapacity;
        internal float StatHandLeftUsage;

        internal float StatHandRightCapacity;
        internal float StatHandRightUsage;

        internal float StatMechCapacity;
        internal float StatMechUsage;
    }

    private enum MinHandReq
    {
        None,
        One,
        Two
    }

    internal const string CarryInHandCollectionId = "CarryInHand";
    internal const string CarryOnMechCollectionId = "CarryOnMech";
    internal const string CarrySharedTopOffCollectionId = "CarrySharedTopOff";

    private static void CalculateCapacity(MechDef mechDef, string collectionId, ChassisLocations location, out float capacity, out float usage, float initialCapacity = 0)
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
            .Aggregate(initialCapacity, ApplyOperation);

        usage = mods
            .Where(m => m.IsUsage)
            .Aggregate(0f, ApplyOperation);
    }
}
