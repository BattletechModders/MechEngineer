using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips
{
    internal class DurabilityStat : IStatHandler
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
