using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using MechEngineer.Features.Globals;
using MechEngineer.Features.OverrideDescriptions;
using TMPro;
using UnityEngine;

namespace MechEngineer.Features.OverrideTonnage;

internal class WeightsHandler : IAdjustTooltipEquipment, IAdjustTooltipWeapon, IAdjustSlotElement
{
    internal static readonly WeightsHandler Shared = new();

    // shared between Weights and DynamicSlots
    public void AdjustTooltipEquipment(TooltipPrefab_Equipment tooltip, MechComponentDef mechComponentDef)
    {
        var reservedSlots = 0;

        if (mechComponentDef.Is<WeightFactors>(out var weightFactors))
        {
            reservedSlots += weightFactors.ReservedSlots;
            var mechDef = Global.ActiveMechDef;
            if (mechDef != null)
            {
                var tonnageChanges = CalculateWeightFactorsChange(mechDef, weightFactors);
                tooltip.tonnageText.text = FormatChanges(mechComponentDef.Tonnage, tonnageChanges);
            }
        }

        if (mechComponentDef.Is<DynamicSlots.DynamicSlots>(out var dynamicSlots))
        {
            reservedSlots += dynamicSlots.ReservedSlots;
        }

        if (reservedSlots > 0)
        {
            tooltip.slotsText.text = $"{mechComponentDef.InventorySize} + {reservedSlots}";
        }
    }

    public void AdjustTooltipWeapon(TooltipPrefab_Weapon tooltip, MechComponentDef mechComponentDef)
    {
        var reservedSlots = 0;

        if (mechComponentDef.Is<WeightFactors>(out var weightFactors))
        {
            reservedSlots += weightFactors.ReservedSlots;
            var mechDef = Global.ActiveMechDef;
            if (mechDef != null)
            {
                var tonnageChanges = CalculateWeightFactorsChange(mechDef, weightFactors);
                tooltip.tonnage.text = FormatChanges(mechComponentDef.Tonnage, tonnageChanges);
            }
        }

        if (mechComponentDef.Is<DynamicSlots.DynamicSlots>(out var dynamicSlots))
        {
            reservedSlots += dynamicSlots.ReservedSlots;
        }

        if (reservedSlots > 0)
        {
            tooltip.slots.text = $"{mechComponentDef.InventorySize} + {reservedSlots}";
        }
    }

    private string FormatChanges(float tonnage, float tonnageChanges)
    {
        if (Mathf.Approximately(tonnage, 0))
        {
            return FloatToText(tonnageChanges);
        }
        else if (Mathf.Approximately(tonnageChanges, 0))
        {
            return tonnage.ToString();
        }
        else
        {
            return $"{tonnage} {FloatToText(tonnageChanges, true)}";
        }
    }

    public void AdjustSlotElement(MechLabItemSlotElement instance, MechLabPanel panel)
    {
        var mechComponentDefNullable = instance.ComponentRef?.Def;
        var weightFactors = mechComponentDefNullable?.GetComponent<WeightFactors>();
        if (weightFactors == null)
        {
            return;
        }
        var mechComponentDef = mechComponentDefNullable!;

        var mechDef = panel.CreateMechDef();
        if (mechDef == null)
        {
            return;
        }

        var tonnageChanges = CalculateWeightFactorsChange(mechDef, weightFactors);
        if (!Mathf.Approximately(tonnageChanges, 0))
        {
            instance.bonusTextA.text = $"{FloatToText(tonnageChanges)} ton";
        }
        else if (instance.bonusTextA.text.EndsWith("ton"))
        {
            instance.bonusTextA.text = mechComponentDef.BonusValueA;
        }
    }

    private static string FloatToText(float number, bool positiveSign = false)
    {
        var sign = positiveSign ? "+ " : "";
        if (number < 0)
        {
            sign = "- ";
            number = -number;
        }

        return sign + number.ToString(OverrideTonnageFeature.settings.MechLabComponentFormat);
    }

    internal static void AdjustInfoWidget(
        MechDef mechDef,
        UIColorRefTracker totalTonnageColor,
        UIColorRefTracker remainingTonnageColor,
        TextMeshProUGUI totalTonnage,
        TextMeshProUGUI remainingTonnage,
        out float currentTonnage)
    {
        var weights = new Weights(mechDef);
        currentTonnage = weights.TotalWeight;

        var precisionHelper = InfoTonnageHelper.KilogramStandard;

        var maxTonnage = mechDef.Chassis.Tonnage;

        if (precisionHelper.IsSmaller(maxTonnage, currentTonnage))
        {
            totalTonnageColor.SetUIColor(UIColor.Red);
            remainingTonnageColor.SetUIColor(UIColor.Red);
        }
        else
        {
            totalTonnageColor.SetUIColor(UIColor.WhiteHalf);
            if (precisionHelper.IsSmaller(maxTonnage, currentTonnage + OverrideTonnageFeature.settings.UnderweightWarningThreshold))
            {
                remainingTonnageColor.SetUIColor(UIColor.White);
            }
            else
            {
                remainingTonnageColor.SetUIColor(UIColor.Gold);
            }
        }

        totalTonnage.SetText($"{InfoTonnageHelper.TonnageStandard.AsString(currentTonnage)} / {maxTonnage}");
        if (precisionHelper.IsSmaller(maxTonnage, currentTonnage, out var tonnageLeft))
        {
            tonnageLeft = Mathf.Abs(tonnageLeft);
            var left = precisionHelper.AsString(tonnageLeft);
            var s = precisionHelper.IsSame(tonnageLeft, 1f) ? "s" : string.Empty;
            remainingTonnage.SetText($"{left} ton{s} overweight");
        }
        else
        {
            var left = precisionHelper.AsString(tonnageLeft);
            var s = precisionHelper.IsSame(tonnageLeft, 1f) ? "s" : string.Empty;
            remainingTonnage.SetText($"{left} ton{s} remaining");
        }

        var layoutTonnage = remainingTonnage.transform.parent;
        {
            var go = layoutTonnage.gameObject;
            var tooltip = go.GetComponent<HBSTooltip>() ?? go.AddComponent<HBSTooltip>();
            string Format(float value)
            {
                return "<b>" + value.ToString(OverrideTonnageFeature.settings.MechLabMechInfoWidgetToolTipFormat) + "</b>";
            }
            tooltip.defaultStateData.SetObject(new BaseDescriptionDef
            {
                Id = "weights",
                Name = "Weights Summary",
                Details =
                    "A mech consists of a chassis which has an internal structure and an outer protective layer called armor." +
                    " The chassis determines the maximum weight in components and armor that can be mounted." +
                    " The technology base of a mech can provide various weight benefits." +
                    (!PrecisionUtils.Equals(weights.Factors.ChassisFactor, 1) ?
                    $"\r\n" +
                    $"\r\n<i>Chassis</i>" +
                    $"\r\n  <i>Capacity</i>" +
                    $"\r\n    {Format(weights.ChassisWeightCapacity)}" +
                    $"\r\n  <i>Standard Capacity</i>" +
                    $"\r\n    {Format(weights.StandardChassisWeightCapacity)}" +
                    $"\r\n  <i>Factor</i>" +
                    $"\r\n    {Format(weights.Factors.ChassisFactor)}"
                    : "") +
                    $"\r\n" +
                    $"\r\n<i>Armor</i>" +
                    $"\r\n  <i>Weight</i>" +
                    $"\r\n    {Format(weights.ArmorWeight)}" +
                    (!PrecisionUtils.Equals(weights.Factors.ArmorFactor, 1) ?
                    $"\r\n  <i>Standard Weight</i>" +
                    $"\r\n    {Format(weights.StandardArmorWeight)}" +
                    $"\r\n  <i>Factor</i>" +
                    $"\r\n    {Format(weights.Factors.ArmorFactor)}"
                    : "") +
                    $"\r\n  <i>Assigned Points</i>" +
                    $"\r\n    {Format(weights.ArmorAssignedPoints)}" +
                    $"\r\n  <i>Points Per Ton</i>" +
                    $"\r\n    {Format(weights.ArmorPerTon)}" +
                    $"\r\n" +
                    $"\r\n<i>Structure</i>" +
                    $"\r\n  <i>Weight</i>" +
                    $"\r\n    {Format(weights.StructureWeight)}" +
                    (!PrecisionUtils.Equals(weights.Factors.StructureFactor, 1) ?
                    $"\r\n  <i>Standard Weight</i>" +
                    $"\r\n    {Format(weights.StandardStructureWeight)}" +
                    $"\r\n  <i>Factor</i>" +
                    $"\r\n    {Format(weights.Factors.StructureFactor)}"
                    : ""),
                    Icon = "uixSvgIcon_quantity"
            });
        }
    }

    private static float CalculateWeightFactorsChange(MechDef mechDef, WeightFactors componentFactors)
    {
        var weights = new Weights(mechDef, false);
        var before = weights.TotalWeight;
        weights.Factors.Combine(componentFactors);
        var after = weights.TotalWeight;
        return after - before;
    }
}