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
        internal static void AdjustTooltip(TooltipPrefab_Equipment tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
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

            var heatDissipation = engineRef.GetEngineHeatDissipation();

            int minHeatSinks, maxHeatSinks;
            Control.calc.CalcHeatSinks(engineDef, out minHeatSinks, out maxHeatSinks);
            var maxAdditionalHeatSinks = maxHeatSinks - minHeatSinks;

            float walkSpeed, runSpeed, TTwalkSpeed;
            Control.calc.CalcSpeeds(engineDef, panel.activeMechDef.Chassis.Tonnage, out walkSpeed, out runSpeed, out TTwalkSpeed);
            var additionalRunSpeed = runSpeed - UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinSprintFactor;
            var gyroWeight = Control.calc.CalcGyroWeight(engineDef);
            var engineWeight = mechComponentDef.Tonnage - gyroWeight;

            var originalText = tooltip.detailText.text;
            tooltip.detailText.text = "";

            var htype = engineRef.IsDHS ? "Double Heat Sinks" : "Standard Heat Sinks";
            tooltip.detailText.text += "<i>" + htype + "</i>" +
                                       "   Internal: <b>" + minHeatSinks + "</b>" +
                                       "   Additional: <b>" + engineRef.AdditionalHeatSinkCount + "</b> / <b>" + maxAdditionalHeatSinks + "</b>";

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

            tooltip.bonusesText.text = string.Format("- {0} Heat / Turn ; + {1} Top Speed", heatDissipation, additionalRunSpeed);
        }
    }
}