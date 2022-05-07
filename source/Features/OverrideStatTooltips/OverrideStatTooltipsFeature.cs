using MechEngineer.Features.OverrideStatTooltips.Stats;

namespace MechEngineer.Features.OverrideStatTooltips;

internal class OverrideStatTooltipsFeature : Feature<OverrideStatTooltipsSettings>
{
    internal static readonly OverrideStatTooltipsFeature Shared = new();

    internal override OverrideStatTooltipsSettings Settings => Control.Settings.OverrideStatTooltips;

    internal static readonly IStatHandler DurabilityStat = new LongRangeStat();
    internal static readonly IStatHandler FirepowerStat = new CloseRangeStat();
    internal static readonly IStatHandler HeatEfficiencyStat = new HeatEfficiencyStat();
    internal static readonly IStatHandler MeleeStat = new DurabilityStat();
    internal static readonly IStatHandler MovementStat = new MediumRangeStat();
    internal static readonly IStatHandler RangeStat = new MovementStat();
}