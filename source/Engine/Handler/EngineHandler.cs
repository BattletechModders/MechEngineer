using BattleTech;
using BattleTech.UI.Tooltips;
using CustomComponents;

namespace MechEngineer
{
    internal class EngineHandler : IAdjustTooltip
    {
        internal static EngineHandler Shared = new EngineHandler();

        public void AdjustTooltip(TooltipPrefab_Equipment tooltipInstance, MechComponentDef mechComponentDef)
        {
            var engineDef = mechComponentDef.GetComponent<EngineCoreDef>();
            if (engineDef == null)
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

            // use standard heat sinks for non-installed fusion core
            if (engine.CoreRef.CoreDef.Def.Description.Id != engineDef.Def.Description.Id)
            {
                engine.CoreRef = new EngineCoreRef(panel.dataManager.GetDefaultEngineHeatSinkDef(), engineDef);
            }
            var engineRef = engine.CoreRef;

            var movement = engineDef.GetMovement(panel.activeMechDef.Chassis.Tonnage);
            
            var tooltip = new TooltipPrefab_EquipmentAdapter(tooltipInstance);
            var originalText = tooltip.detailText.text;
            tooltip.detailText.text = "";

            foreach (var heatSinkDef in panel.dataManager.GetAllEngineHeatSinkDefs())
            {
                var query = engineRef.Query(heatSinkDef);

                if (query.Count == 0)
                {
                    continue;
                }

                if (Control.settings.AllowMixingHeatSinkTypes || query.IsType)
                {

                    tooltip.detailText.text += "<i>" + heatSinkDef.FullName + "</i>" +
                                               "   Internal: <b>" + query.InternalCount + "</b>" +
                                               "   Additional: <b>" + query.AdditionalCount + "</b> / <b>" + engineDef.MaxAdditionalHeatSinks + "</b>" +
                                               "\r\n";
                }
            }

            tooltip.detailText.text += "<i>Speeds</i>" +
                                       "   Cruise <b>" + movement.WalkSpeed + "</b>" +
                                       " / Top <b>" + movement.RunSpeed + "</b>";

            tooltip.detailText.text += "\r\n" +
                                       "<i>Weights [Ton]</i>" +
                                       "   Engine: <b>" + engine.EngineTonnage + "</b>" +
                                       "   Gyro: <b>" + engine.GyroTonnage + "</b>" +
                                       "   Sinks: <b>" + engine.HeatSinkTonnage + "</b>";

            tooltip.tonnageText.text = $"{engine.TotalTonnage}";

            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += originalText;
            tooltip.detailText.SetAllDirty();

            tooltip.bonusesText.text = engineRef.BonusValueA;
            tooltip.bonusesText.SetAllDirty();
        }
    }
}