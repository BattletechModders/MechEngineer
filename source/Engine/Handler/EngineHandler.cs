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

            float walkSpeed, runSpeed;
            Control.calc.CalcSpeeds(engineDef, panel.activeMechDef.Chassis.Tonnage, out walkSpeed, out runSpeed);

            var originalText = tooltip.detailText.text;
            tooltip.detailText.text = "";

            foreach (var hstype in HeatSinkTypeExtensions.Types())
            {
                var query = engineRef.Query(hstype);

                if (Control.settings.AllowMixingHeatSinkTypes || query.IsType)
                {
                    
                    tooltip.detailText.text += "<i>" + hstype.Full() + "</i>" +
                                               "   Internal: <b>" + query.InternalCount + "</b>" +
                                               "   Additional: <b>" + query.AdditionalCount + "</b> / <b>" + engineDef.MaxAdditionalHeatSinks + "</b>";
                }
            }

            tooltip.detailText.text += "\r\n" +
                                       "<i>Speeds</i>" +
                                       "   Cruise <b>" + walkSpeed + "</b>" +
                                       " / Top <b>" + runSpeed + "</b>";

            tooltip.detailText.text += "\r\n" +
                                       "<i>Weights</i>" +
                                       "   Engine: <b>" + engine.EngineTonnage + "</b> Ton" +
                                       "   Gyro: <b>" + engine.CoreDef.GyroTonnage + "</b> Ton" +
                                       "   Sinks: <b>" + engineRef.HeatSinkTonnage + "</b> Ton";

            tooltip.tonnageText.text = string.Format("{0}", engine.Tonnage);

            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += originalText;
            tooltip.detailText.SetAllDirty();

            tooltip.bonusesText.text = engineRef.BonusValueA;
            tooltip.bonusesText.SetAllDirty();
        }
    }
}