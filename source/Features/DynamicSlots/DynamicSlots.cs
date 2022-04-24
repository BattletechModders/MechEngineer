using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using MechEngineer.Features.OverrideDescriptions;
using MechEngineer.Features.OverrideTonnage;
using MechEngineer.Helper;
using TMPro;

namespace MechEngineer.Features.DynamicSlots;

[CustomComponent("DynamicSlots")]
public class DynamicSlots : SimpleCustomComponent, IAdjustTooltipEquipment, IAdjustTooltipWeapon
{
    public int ReservedSlots { get; set; }

    public bool InnerAdjacentOnly { get; set; } = false;

    public bool? ShowIcon { get; set; } = DynamicSlotsFeature.settings.DefaultShowIcon;
    public bool? ShowFixedEquipmentOverlay { get; set; } = DynamicSlotsFeature.settings.DefaultShowFixedEquipmentOverlay;

    public string NameText { get; set; } = DynamicSlotsFeature.settings.DefaultNameText;
    public string DefaultBonusATextIfReservedSlot { get; set; } = DynamicSlotsFeature.settings.DefaultBonusATextIfReservedSlot;
    public string DefaultBonusATextIfMovableSlot { get; set; } = DynamicSlotsFeature.settings.DefaultBonusATextIfMovableSlot;
    public string BonusBText { get; set; } = DynamicSlotsFeature.settings.DefaultBonusBText;
    public string BackgroundColor { get; set; } = DynamicSlotsFeature.settings.DefaultBackgroundColor;

    public void AdjustTooltipEquipment(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef)
    {
        WeightsHandler.Shared.AdjustTooltipEquipment(tooltip, componentDef);
    }

    public void AdjustTooltipWeapon(TooltipPrefab_Weapon tooltip, MechComponentDef componentDef)
    {
        WeightsHandler.Shared.AdjustTooltipWeapon(tooltip, componentDef);
    }

    internal void ApplyTo(MechLabItemSlotElement element, bool isReservedSlot)
    {
        void SetText(string text, TextMeshProUGUI textMesh)
        {
            if (text == "")
            {
                textMesh.gameObject.SetActive(false);
            }
            else if (text != null)
            {
                textMesh.text = text;
                textMesh.gameObject.SetActive(true);
            }
        }

        SetText(NameText, element.nameText);
        SetText(
            isReservedSlot ? DefaultBonusATextIfReservedSlot : DefaultBonusATextIfMovableSlot,
            element.bonusTextA
        );
        SetText(BonusBText, element.bonusTextB);

        if (!string.IsNullOrEmpty(BackgroundColor))
        {
            element.backgroundColor.SetColorFromString(BackgroundColor);
        }

        if (ShowIcon.HasValue)
        {
            element.icon.gameObject.SetActive(ShowIcon.Value);
        }

        if (ShowFixedEquipmentOverlay.HasValue)
        {
            element.fixedEquipmentOverlay.gameObject.SetActive(ShowFixedEquipmentOverlay.Value);
        }
    }
}