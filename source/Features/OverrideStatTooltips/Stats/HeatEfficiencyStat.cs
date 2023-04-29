using BattleTech;
using Localize;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Stats;

internal class HeatEfficiencyStat : IStatHandler
{
    public void SetupTooltip(StatTooltipData tooltipData, MechDef mechDef)
    {
        var stats = new MechDefHeatEfficiencyStatistics(mechDef);

        tooltipData.dataList.Add("<u>" + Strings.T("Heat Sinking") + "</u>", Strings.T("{0} Heat", - stats.HeatSinking + stats.EndMoveHeat));
        tooltipData.dataList.Add(Strings.T("End of Turn"), Strings.T("{0} Heat", - stats.HeatSinking));
        tooltipData.dataList.Add(Strings.T("End of Movement"), Strings.T("{0} Heat", stats.EndMoveHeat));
        tooltipData.dataList.Add("<u>" + Strings.T("Alpha Strike") + "</u>", Strings.T("{0} Heat", stats.AlphaStrike));
        tooltipData.dataList.Add(Strings.T("Heat Delta"), Strings.T("{0} Heat", stats.AlphaStrike - stats.HeatSinking + stats.EndMoveHeat));
        tooltipData.dataList.Add(Strings.T("Jumping"), Strings.T("{0} Heat", stats.JumpHeat));
        tooltipData.dataList.Add(Strings.T("Heat Levels"), Strings.T("{0} / {1} Heat", stats.Overheat, stats.MaxHeat));
    }

    public float BarValue(MechDef mechDef)
    {
        var stats = new MechDefHeatEfficiencyStatistics(mechDef);
        return stats.GetStatisticRating();
    }
}