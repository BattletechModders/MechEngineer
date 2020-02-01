namespace MechEngineer.Features.OverrideStatTooltips
{
    internal class OverrideStatTooltipsFeature : Feature<OverrideStatTooltipsSettings>
    {
        internal static OverrideStatTooltipsFeature Shared = new OverrideStatTooltipsFeature();

        internal override OverrideStatTooltipsSettings Settings => Control.settings.OverrideStatTooltips;

        internal static IStatHandler DurabilityStat = new LongRangeStat();
        internal static IStatHandler FirepowerStat = new CloseRangeStat();
        internal static IStatHandler HeatEfficiencyStat = new HeatEfficiencyStat();
        internal static IStatHandler MeleeStat = new DurabilityStat();
        internal static IStatHandler MovementStat = new MediumRangeStat();
        internal static IStatHandler RangeStat = new MovementStat();
    }
}
