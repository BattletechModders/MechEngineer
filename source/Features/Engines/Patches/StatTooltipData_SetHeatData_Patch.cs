using System;
using BattleTech;
using Harmony;
using Localize;
using MechEngineer.Features.Engines.Helper;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(StatTooltipData), "SetHeatData")]
    public static class StatTooltipData_SetHeatData_Patch
    {
        public static void Postfix(StatTooltipData __instance, MechDef def)
        {
            try
            {
                var mechDef = def;
                var stats = new MechDefHeatEfficiencyStatistics(mechDef);

                var tooltipData = __instance;
                void Replace(string text, string value, string newText = null)
                {
                    tooltipData.dataList.Remove(text);
                    tooltipData.dataList.Add(newText ?? text, value);
                }

                Replace(Strings.T("Heat Sinking"), Strings.T("{0} Heat", stats.HeatSinking), "<u>" + Strings.T("Heat Sinking") + "</u>");
                Replace(Strings.T("Alpha Strike"), Strings.T("{0} Heat", stats.AlphaStrike), "<u>" + Strings.T("Alpha Strike") + "</u>");
                Replace(Strings.T("Avg. Jump Heat"), Strings.T("{0} Heat", stats.JumpHeat), Strings.T("Jump Heat"));
                Replace(Strings.T("Shutdown"), Strings.T("{0} / {1} Heat", stats.Overheat, stats.MaxHeat), Strings.T("Heat Levels"));
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}