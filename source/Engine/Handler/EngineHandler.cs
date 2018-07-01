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
            var engineDef = mechComponentDef.GetEngineCoreDef();
            if (engineDef == null)
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

            if (Control.settings.AllowMixingDoubleAndSingleHeatSinks || engineRef.IsSHS)
            {
                tooltip.detailText.text += "<i>Standard Heat Sinks</i>" +
                                           "   Internal: <b>" + engineRef.InternalSHSCount + "</b>" +
                                           "   Additional: <b>" + engineRef.AdditionalSHSCount + "</b> / <b>" + engineDef.MaxAdditionalHeatSinks + "</b>";
            }

            if (Control.settings.AllowMixingDoubleAndSingleHeatSinks || engineRef.IsDHS)
            {
                tooltip.detailText.text += "<i>Double Heat Sinks</i>" +
                                           "   Internal: <b>" + engineRef.InternalDHSCount + "</b>" +
                                           "   Additional: <b>" + engineRef.AdditionalDHSCount + "</b> / <b>" + engineDef.MaxAdditionalHeatSinks + "</b>";
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