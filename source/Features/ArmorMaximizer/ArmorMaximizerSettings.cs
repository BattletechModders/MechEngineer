namespace MechEngineer.Features.ArmorMaximizer;

public class ArmorMaximizerSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Max Armor button works within CBT ratios.";

	public bool IgnoreHeadByDefault { get; set; } = false;
	public string IgnoreHeadByDefaultDescription => "When maximizing armor, head armor will be ignored. (Not yet supported, thinking about locking any armor location via UI)";

	public float TorsoFrontBackRatio { get; set; } = 0.69f;
}