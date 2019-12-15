using BattleTech;
using BattleTech.UI.Tooltips;
using CustomComponents;
using MechEngineer.Features.Globals;
using MechEngineer.Features.OverrideDescriptions;

namespace MechEngineer.Features.DynamicSlots
{
    [CustomComponent("DynamicSlotPlaceholder")]
    public class DynamicSlotPlaceholder : SimpleCustomComponent, IAdjustTooltip
    {
        public bool ShowFixedEquipmentOverlay { get; set; } = true;

        public void AdjustTooltip(TooltipPrefab_Equipment tooltipInstance, MechComponentDef componentDef)
        {
            var placeholder = componentDef.GetComponent<DynamicSlotPlaceholder>();
            if (placeholder == null)
            {
                return;
            }

            var panel = Global.ActiveMechLabPanel;
            if (panel == null)
            {
                return;
            }

            var builder = new MechDefBuilder(panel.activeMechDef);

            var tooltip = new TooltipPrefab_EquipmentAdapter(tooltipInstance);
            tooltip.tonnageText.text = "-";

            var originalText = tooltip.detailText.text;
            tooltip.detailText.text = "";
            tooltip.detailText.text += $"<i>Critical Slots</i>   Total: <b>{builder.TotalMax}</b>   Free: <b>{builder.TotalFree}</b>";

            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += "<i>Dynamic Slots:</i>";
            foreach (var reservedSlots in builder.DynamicSlots)
            {
                tooltip.detailText.text += "\r\n";
                tooltip.detailText.text += $"      <b><color=#F79B26FF>{reservedSlots.Def.Description.UIName}</color> {reservedSlots.ReservedSlots}</b>";
            }
            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += originalText;
            tooltip.detailText.SetAllDirty();
        }
    }
}
