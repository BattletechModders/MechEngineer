namespace MechEngineer.Features.ArmorMaximizer;

public class ArmorMaximizerSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Max Armor button works within CBT ratios.";

	public bool HeadPointsUnChanged { get; set; } = true;
	public float TorsoFrontBackRatio { get; set; } = 0.69f;
}