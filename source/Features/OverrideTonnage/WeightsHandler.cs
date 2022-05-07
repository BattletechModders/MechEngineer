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
                var tonnageChanges = Weights.CalculateWeightFactorsChange(mechDef, weightFactors);
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
                var tonnageChanges = Weights.CalculateWeightFactorsChange(mechDef, weightFactors);
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

        var tonnageChanges = Weights.CalculateWeightFactorsChange(mechDef, weightFactors);
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
        return $"{sign}{number:0.##}";
    }

    internal static void AdjustInfoWidget(
        MechDef mechDef,
        UIColorRefTracker totalTonnageColor,
        UIColorRefTracker remainingTonnageColor,
        TextMeshProUGUI totalTonnage,
        TextMeshProUGUI remainingTonnage,
        out float currentTonnage)
    {
        currentTonnage = Weights.CalculateTotalTonnage(mechDef);

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
    }
}