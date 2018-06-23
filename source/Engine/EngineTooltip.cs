using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;

namespace MechEngineMod
{
    internal static class EngineTooltip
    {
        // show extended engine information (as its now shown anywhere else)
        internal static void AdjustTooltip(TooltipPrefab_EquipmentAdapter tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
        {
            var engineDef = mechComponentDef.GetEngineDef();
            if (engineDef == null)
            {
                return;
            }

            var engineRef = panel.activeMechInventory.Select(x => x.GetEngineRef()).FirstOrDefault(x => x != null);
            if (engineRef == null)
            {
                return;
            }

            float walkSpeed, runSpeed;
            Control.calc.CalcSpeeds(engineDef, panel.activeMechDef.Chassis.Tonnage, out walkSpeed, out runSpeed);
            var gyroWeight = Control.calc.CalcGyroWeight(engineDef);
            var engineWeight = mechComponentDef.Tonnage - gyroWeight;

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
                                       "   Engine: <b>" + engineWeight + "</b> Ton" +
                                       "   Gyro: <b>" + gyroWeight + "</b> Ton";

            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += originalText;
            tooltip.detailText.SetAllDirty();

            tooltip.bonusesText.text = engineRef.BonusValueA;
            tooltip.bonusesText.SetAllDirty();
        }
    }
}