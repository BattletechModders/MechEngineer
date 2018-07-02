using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal partial class EngineHandler
    {
        internal static EngineHandler Shared = new EngineHandler();
    }

    internal partial class EngineHandler : ITonnageChanges
    {
        public float TonnageChanges(MechDef mechDef)
        {
            var engine = mechDef.GetEngine();
            if (engine == null)
            {
                return 0;
            }

            return engine.TonnageChanges;
        }
    }

    internal partial class EngineHandler : IAdjustTooltip
    {
        public void AdjustTooltip(TooltipPrefab_EquipmentAdapter tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
        {
            if (!(mechComponentDef is EngineCoreDef engineDef))
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

            Control.calc.CalcSpeeds(engineDef, panel.activeMechDef.Chassis.Tonnage, out var walkSpeed, out var runSpeed);

            var originalText = tooltip.detailText.text;
            tooltip.detailText.text = "";

            foreach (var heatSinkDef in mechComponentDef.DataManager.GetAllEngineHeatSinkDefs())
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
                                       "   Cruise <b>" + walkSpeed + "</b>" +
                                       " / Top <b>" + runSpeed + "</b>";

            tooltip.detailText.text += "\r\n" +
                                       "<i>Weights [Ton]</i>" +
                                       "   Engine: <b>" + engine.EngineTonnage + "</b>" +
                                       "   Gyro: <b>" + engine.CoreDef.GyroTonnage + "</b>" +
                                       "   Sinks: <b>" + engineRef.HeatSinkTonnage + "</b>";

            tooltip.tonnageText.text = $"{engine.Tonnage}";

            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += originalText;
            tooltip.detailText.SetAllDirty();

            tooltip.bonusesText.text = engineRef.BonusValueA;
            tooltip.bonusesText.SetAllDirty();
        }
    }
}