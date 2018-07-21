using System.Collections.Generic;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    internal partial class EngineHandler
    {
        internal static EngineHandler Shared = new EngineHandler();
    }

    internal partial class EngineHandler : IAdjustTooltip
    {
        public void AdjustTooltip(TooltipPrefab_EquipmentAdapter tooltip, MechComponentDef mechComponentDef)
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

            engine.CoreDef = engineDef; // overwrite the core def for better tooltip
            var engineRef = engine.CoreRef;

            var movement = engineDef.GetMovement(panel.activeMechDef.Chassis.Tonnage);

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