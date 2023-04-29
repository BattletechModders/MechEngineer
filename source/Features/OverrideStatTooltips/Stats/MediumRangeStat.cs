using BattleTech;
using Localize;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Stats;

internal class MediumRangeStat : IStatHandler
{
    public void SetupTooltip(StatTooltipData tooltipData, MechDef mechDef)
    {
        var firepower = GetFirepower(mechDef);
        tooltipData.dataList.Add("<u>" + Strings.T("Medium Range Dmg") + "</u>", $"{firepower.TotalDamage}");
        tooltipData.dataList.Add(Strings.T("Instability Damage"), $"{firepower.TotalInstability}");
        tooltipData.dataList.Add(Strings.T("Heat Damage"), $"{firepower.TotalHeatDamage}");
        tooltipData.dataList.Add(Strings.T("Structure Damage"), $"{firepower.TotalStructureDamage}");
        tooltipData.dataList.Add(Strings.T("Average Accuracy"), $"{-firepower.AverageAccuracy}");
    }

    public float BarValue(MechDef mechDef)
    {
        var firepower = GetFirepower(mechDef);
        return firepower.BarValue(firepower.TotalDamage);
    }

    private MechDefFirepowerStatistics GetFirepower(MechDef mechDef)
    {
        return new(
            mechDef,
            OverrideStatTooltipsFeature.Shared.Settings.CloseRangeMax + 1,
            OverrideStatTooltipsFeature.Shared.Settings.MediumRangeMax
        );
    }
}