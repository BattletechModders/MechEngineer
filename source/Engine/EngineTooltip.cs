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

            var htype = engineRef.IsDHS ? "Double Heat Sinks" : "Standard Heat Sinks";
            tooltip.detailText.text += "<i>" + htype + "</i>" +
                                       "   Internal: <b>" + engineDef.MinHeatSinks + "</b>" +
                                       "   Additional: <b>" + engineRef.AdditionalHeatSinkCount + "</b> / <b>" + engineDef.MaxAdditionalHeatSinks + "</b>";

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