using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.Globals;
using MechEngineer.Features.OverrideDescriptions;

namespace MechEngineer.Features.Engines
{
    [CustomComponent("EngineHeatBlock")]
    public class EngineHeatBlockDef : SimpleCustom<HeatSinkDef>, IAdjustTooltip, IAdjustSlotElement
    {
        public int HeatSinkCount { get; set; }
        
        public void AdjustTooltip(TooltipPrefab_Equipment tooltipInstance, MechComponentDef mechComponentDef)
        {
            var def = mechComponentDef.GetComponent<EngineHeatBlockDef>();
            if (def == null)
            {
                return;
            }

            var panel = Global.ActiveMechLabPanel;
            if (panel == null)
            {
                return;
            }

            var engine = panel.activeMechInventory.GetEngine();
            if (engine == null)
            {
                return;
            }

            engine.HeatBlockDef = def;

            var tooltip = new TooltipPrefab_EquipmentAdapter(tooltipInstance);
            var originalText = tooltip.detailText.text;
            tooltip.detailText.text = "";
            
            tooltip.detailText.text += $"<i>{engine.EngineHeatSinkDef.FullName}</i>" +
                                       $"\r\n   Internal" +
                                       $"   Free: <b>{engine.CoreDef.InternalHeatSinkCount}</b> " +
                                       $"   Additional: <b>{engine.HeatBlockDef.HeatSinkCount} / {engine.CoreDef.InternalHeatSinkAdditionalMaxCount}</b>" +
                                       $"\r\n   External" +
                                       $"   Free: <b>{engine.ExternalHeatSinkFreeCount} / {engine.CoreDef.ExternalHeatSinkFreeMaxCount}</b> " +
                                       $"   Additional: <b>{engine.ExternalHeatSinkAdditionalCount}</b>" +
                                       "\r\n";

            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += originalText;
            tooltip.detailText.SetAllDirty();

            tooltip.bonusesText.text = BonusValueEngineHeatDissipation(engine) + ", " + BonusValueEngineHeatSinkCounts(engine);
            tooltip.bonusesText.SetAllDirty();
        }

        public void AdjustSlotElement(MechLabItemSlotElement instance, MechLabPanel panel)
        {
            var def = instance.ComponentRef.GetComponent<EngineHeatBlockDef>();
            if (def == null)
            {
                return;
            }

            var engine = panel.activeMechDef?.Inventory.GetEngine();
            if (engine == null)
            {
                return;
            }

            var adapter = new MechLabItemSlotElementAdapter(instance);
            adapter.bonusTextA.text = BonusValueEngineHeatDissipation(engine);
            adapter.bonusTextB.text = BonusValueEngineHeatSinkCounts(engine);
        }

        private static string BonusValueEngineHeatDissipation(Engine engine)
        {
            return $"- {engine.EngineHeatDissipation} Heat";
        }

        private static string BonusValueEngineHeatSinkCounts(Engine engine)
        {
            return $"{engine.EngineHeatSinkDef.Abbreviation} {engine.CoreDef.InternalHeatSinkCount + engine.HeatBlockDef.HeatSinkCount}";
        }
    }
}
