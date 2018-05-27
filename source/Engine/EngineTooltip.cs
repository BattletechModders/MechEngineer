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
            float walkSpeed, runSpeed, TTwalkSpeed;
            Control.calc.CalcSpeeds(engineDef, panel.activeMechDef.Chassis.Tonnage, out walkSpeed, out runSpeed, out TTwalkSpeed);
            var gyroWeight = Control.calc.CalcGyroWeight(engineDef);
            var engineWeight = mechComponentDef.Tonnage - gyroWeight;

            tooltip.detailText.text = "walkSpeed=" + walkSpeed + "\r\n"
                                      + "runSpeed=" + runSpeed + "\r\n"
                                      + "TTwalkSpeed=" + TTwalkSpeed + "\r\n"
                                      + "minHeatSinks=" + minHeatSinks + "\r\n"
                                      + "maxHeatSinks=" + maxHeatSinks + "\r\n"
                                      + "dhs=" + engineRef.IsDHS + "\r\n"
                                      + "dhsSinks=" + engineRef.AdditionalDHSCount + "\r\n"
                                      + "shsSinks=" + engineRef.AdditionalSHSCount + "\r\n"
                                      + "engineWeight=" + engineWeight + "\r\n"
                                      + "gyroWeight=" + gyroWeight + "\r\n"
                                      + "------- \r\n";

            tooltip.bonusesText.text = string.Format("- {0} Heat / Turn", heatDissipation);
        }
    }
}