namespace MechEngineer.Features.AccuracyEffects;

public class AccuracyEffectsSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Enables statistic effects for arm accuracy.";

    public string CombatHUDTooltipName { get; set; } = "WEAPON MOUNT";
    public string CombatHUDTooltipNameDescription => "The name to call the ARM MOUNTED accuracy bonus in the combat tooltips.";
}