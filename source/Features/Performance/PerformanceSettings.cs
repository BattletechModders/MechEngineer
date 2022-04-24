namespace MechEngineer.Features.Performance;

internal class PerformanceSettings : ISettings
{
    public bool Enabled { get; set; } = false;
    public string EnabledDescription => "Some performance patches to the vanilla game. Could interfere with other performance patches from other mods.";
}