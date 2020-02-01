using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips
{
    internal class LongRangeStat : IStatHandler
    {
        public void SetupTooltip(StatTooltipData tooltipData, MechDef mechDef)
        {
            tooltipData.dataList.Clear();
        }

        public float BarValue(MechDef mechDef)
        {
            return 0.5f;
        }
    }
}
