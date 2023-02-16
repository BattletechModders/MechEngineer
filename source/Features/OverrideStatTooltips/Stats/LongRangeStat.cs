using BattleTech;
using Localize;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Stats;

internal class LongRangeStat : IStatHandler
{
    public void SetupTooltip(StatTooltipData tooltipData, MechDef mechDef)
    {
        tooltipData.dataList.Clear();

        {
            var firepower = GetLongRangeFirepower(mechDef);
            tooltipData.dataList.Add("<u>" + Strings.T("Long Range Dmg") + "</u>", $"{firepower.TotalDamage}");
        }
        {
            var firepower = GetExtremeLongRangeFirepower(mechDef);
            tooltipData.dataList.Add("<u>" + Strings.T("Very Long Range Dmg") + "</u>", $"{firepower.TotalDamage}");
        }
        {
            var firepower = GetIndirectFirepower(mechDef);
            // TODO apply range limits and add copy to medium range stat
            tooltipData.dataList.Add(Strings.T("Indirect Range Dmg"), $"{firepower.TotalDamage}");
        }
        {
            var firepower = GetFirepower(mechDef);
            tooltipData.dataList.Add(Strings.T("Stability Dmg"), $"{firepower.TotalInstability}");
            tooltipData.dataList.Add(Strings.T("Heat / Struct.Dmg"), $"{firepower.TotalHeatDamage} / {firepower.TotalStructureDamage}");
            tooltipData.dataList.Add(Strings.T("Average Accuracy"), $"{-firepower.AverageAccuracy}");
        }
    }

    public float BarValue(MechDef mechDef)
    {
        var firepower = GetFirepower(mechDef);
        return firepower.BarValue(firepower.TotalDamage);
    }

    private MechDefFirepowerStatistics GetIndirectFirepower(MechDef mechDef)
    {
        return new(mechDef, (x) => x.IndirectFireCapable());
    }

    private MechDefFirepowerStatistics GetExtremeLongRangeFirepower(MechDef mechDef)
    {
        return new(
            mechDef,
            OverrideStatTooltipsFeature.Shared.Settings.LongRangeMax + 1,
            int.MaxValue
        );
    }

    private MechDefFirepowerStatistics GetLongRangeFirepower(MechDef mechDef)
    {
        return new(
            mechDef,
            OverrideStatTooltipsFeature.Shared.Settings.MediumRangeMax + 1,
            OverrideStatTooltipsFeature.Shared.Settings.LongRangeMax
        );
    }

    private MechDefFirepowerStatistics GetFirepower(MechDef mechDef)
    {
        return new(
            mechDef,
            OverrideStatTooltipsFeature.Shared.Settings.MediumRangeMax + 1,
            int.MaxValue
        );
    }
}