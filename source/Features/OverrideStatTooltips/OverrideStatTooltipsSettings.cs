namespace MechEngineer.Features.OverrideStatTooltips;

public class OverrideStatTooltipsSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Overrides stat tooltips of mechs including 'progress bar' summaries.";

    public string MovementTitleText = "Medium Range";
    public string HeatEfficiencyTitleText = "Heat Efficiency";
    public string DurabilityTitleText = "Long Range";
    public string AvgRangeTitleText = "Movement";
    public string MeleeTitleText = "Durability";
    public string FirepowerTitleText = "Close Range";

    public int CloseRangeMax = 89;
    public int MediumRangeMax = 359;
    public int LongRangeMax = 569;
}