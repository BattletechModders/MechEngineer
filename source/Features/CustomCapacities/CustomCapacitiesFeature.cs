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
        Settings.Complete();
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
            hasError = context.IsHandOverweight || context.IsHandMissingFreeHand || context.IsLeftOverMissing;
            description = new(customCapacity.Description);
            bool ReqString(MinHandReq req, out string? text)
            {
                text = req switch
                {
                    MinHandReq.None => null,
                    MinHandReq.One => "one-handed",
                    MinHandReq.Two => "two-handed req.",
                    _ => throw new ArgumentOutOfRangeException()
                };
                return text != null;
            }
            void WeightErrorColor(bool condition, out string prefix, out string postfix)
            {
                if (condition)
                {
                    prefix = "<color=#F04228FF>";
                    postfix = "</color>";
                }
                else
                {
                    prefix = "";
                    postfix = "";
                }
            }
            WeightErrorColor(context.IsHandOverweight, out var thPre, out var thPost);
            WeightErrorColor(context.IsLeftHandOverweight && context.IsHandMissingFreeHand, out var lhPre, out var lhPost);
            WeightErrorColor(context.IsRightHandOverweight && context.IsHandMissingFreeHand, out var rhPre, out var rhPost);
            WeightErrorColor(context.IsLeftOverMissing, out var loPre, out var loPost);

            description.Details +=
                $"\r\n" +
                $"\r\n<i>Total</i>" +
                $"\r\n  usage {thPre}<b>{context.HandTotalUsage:0.###} / {context.HandTotalCapacity:0.###}</b>{thPost}" +
                $"\r\n" +
                $"\r\n<i>Left Hand</i>" +
                $"\r\n  usage {lhPre}<b>{context.HandLeftUsage:0.###} / {context.HandLeftCapacity:0.###}</b>{lhPost}" +
                (ReqString(context.LeftHandReq, out var leftHandReqText) ?
                $"\r\n  {leftHandReqText}"
                : "") +
                $"\r\n" +
                $"\r\n<i>Right Hand</i>" +
                $"\r\n  usage {rhPre}<b>{context.HandRightUsage:0.###} / {context.HandRightCapacity:0.###}</b>{rhPost}" +
                (ReqString(context.RightHandReq, out var rightHandReqText) ?
                $"\r\n  {rightHandReqText}"
                : "") +
                $"\r\n" +
                (context.HasLeftOverTopOff ?
                $"\r\n<i>Left Over</i>" +
                $"\r\n  usage {loPre}<b>{context.LeftOverUsage:0.##} / {context.LeftOverCapacity:0.###}</b>{loPost}"
                : "");
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
        foreach (var customCapacity in Settings.AllCapacities)
        {
            if (customCapacity == Shared.Settings.CarryWeight)
            {
                ValidateCarryWeight(mechDef, errors);
            }
            else
            {
                CalculateCapacity(
                    mechDef,
                    customCapacity.Description.Id,
                    ChassisLocations.All,
                    out var capacity,
                    out var usage
                );
                var hasError = PrecisionUtils.SmallerThan(capacity, usage);
                if (hasError)
                {
                    errors.Add(MechValidationType.InvalidInventorySlots, customCapacity.ErrorOverweight);
                }
            }
        }
    }

    // Carry Capacity - TacOps p.92
    // HandHeld Weapons - TacOps p.316
    private void ValidateCarryWeight(MechDef mechDef, Errors errors)
    {
        var context = CalculateCarryWeight(mechDef);

        if (context.IsHandOverweight || context.IsLeftOverMissing)
        {
            errors.Add(MechValidationType.Overweight, Settings.CarryWeight!.ErrorOverweight);
        }
        else if (context.IsHandMissingFreeHand)
        {
            errors.Add(MechValidationType.Overweight, Settings.CarryHandErrorOneFreeHand);
        }
    }

    private CarryContext CalculateCarryWeight(MechDef mechDef)
    {
        var context = new CarryContext();
        CalculateCarryInHand(mechDef, context);
        CalculateCarryLeftOver(mechDef, context);
        return context;
    }

    private void CalculateCarryLeftOver(MechDef mechDef, CarryContext context)
    {
        CalculateCapacity(
            mechDef,
            CarryLeftOverTopOffCollectionId,
            ChassisLocations.All,
            out context.LeftOverTopOff,
            out _,
            Settings.CarryLeftOverTopOff * mechDef.Chassis.Tonnage
        );

        CalculateCapacity(
            mechDef,
            CarryLeftOverCollectionId,
            ChassisLocations.All,
            out context.LeftOverCapacity,
            out context.LeftOverUsage,
            Mathf.Max(0, context.LeftOverTopOff - context.HandTotalUsage),
            context
        );
    }

    private static void CalculateCarryInHand(MechDef mechDef, CarryContext context)
    {
        CalculateCapacity(
            mechDef,
            CarryInHandCollectionId,
            ChassisLocations.LeftArm,
            out context.HandLeftCapacity,
            out context.HandLeftUsage
        );

        CalculateCapacity(
            mechDef,
            CarryInHandCollectionId,
            ChassisLocations.RightArm,
            out context.HandRightCapacity,
            out context.HandRightUsage
        );
    }

    private class CarryContext
    {
        internal bool IsHandOverweight => PrecisionUtils.SmallerThan(HandTotalCapacity, HandTotalUsage);
        internal bool IsLeftOverMissing => PrecisionUtils.SmallerThan(LeftOverCapacity, LeftOverUsage);

        internal bool IsLeftHandOverweight => PrecisionUtils.SmallerThan(HandLeftCapacity, HandLeftUsage);
        internal bool IsRightHandOverweight => PrecisionUtils.SmallerThan(HandRightCapacity, HandRightUsage);

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

        internal float TotalCapacity => LeftOverTopOff > HandTotalCapacity ? LeftOverTopOff : HandTotalCapacity;
        internal float TotalUsage => HandTotalUsage + LeftOverUsage;

        internal float HandTotalCapacity => HandLeftCapacity + HandRightCapacity;
        internal float HandTotalUsage => HandLeftUsage + HandRightUsage;

        internal float HandLeftCapacity;
        internal float HandLeftUsage;

        internal float HandRightCapacity;
        internal float HandRightUsage;

        internal bool HasLeftOverTopOff => !PrecisionUtils.Equals(0, LeftOverTopOff);

        internal float LeftOverTopOff;
        internal float LeftOverCapacity;
        internal float LeftOverUsage;
    }

    private enum MinHandReq
    {
        None,
        One,
        Two
    }

    internal const string HeatSinkCollectionId = "HeatSink";
    internal const string HeatSinkEngineAdditionalCollectionId = "HeatSinkEngineAdditional";
    internal const string CarryInHandCollectionId = "CarryInHand";
    internal const string CarryLeftOverCollectionId = "CarryLeftOver";
    internal const string CarryLeftOverTopOffCollectionId = "CarryLeftOverTopOff";

    private static void CalculateCapacity(MechDef mechDef, string collectionId, ChassisLocations location, out float capacity, out float usage, float initialCapacity = 0, CarryContext? context = null)
    {
        var mods = mechDef.Inventory
            .SelectMany(r =>
                r.GetComponents<CapacityModCustom>()
                .Where(mod => mod.Collection == collectionId)
                .Where(mod => !mod.IsLocationRestricted || (r.MountedLocation & location) != ChassisLocations.None)
            )
            .OrderBy(m => m.IsUsage)
            .ThenBy(m => m.Priority)
            .ThenBy(m => m.Operation)
            .ToList();

        var quantityCapacityFactor = 0f;
        float ApplyOperation(float previous, CapacityModCustom mod)
        {
            var factor = mod.QuantityFactorType switch
            {
                QuantityFactorType.One => 1f,
                QuantityFactorType.Capacity => quantityCapacityFactor,
                QuantityFactorType.CarryLeftOverTopOff => context!.LeftOverTopOff,
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
        quantityCapacityFactor = capacity;

        usage = mods
            .Where(m => m.IsUsage)
            .Aggregate(0f, ApplyOperation);
    }
}
