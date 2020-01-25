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
                tooltipData.dataList.Clear();

                tooltipData.dataList.Add("<u>" + Strings.T("Heat Sinking") + "</u>", Strings.T("{0} Heat", stats.HeatSinking));
                tooltipData.dataList.Add("<u>" + Strings.T("Alpha Strike") + "</u>", Strings.T("{0} Heat", stats.AlphaStrike));
                tooltipData.dataList.Add(Strings.T("Jump Heat"), Strings.T("{0} Heat", stats.JumpHeat));
                tooltipData.dataList.Add(Strings.T("Heat Levels"), Strings.T("{0} / {1} Heat", stats.Overheat, stats.MaxHeat));
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}