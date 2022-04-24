using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips;

internal interface IStatHandler
{
    public void SetupTooltip(StatTooltipData tooltipData, MechDef mechDef);
    public float BarValue(MechDef mechDef);
}