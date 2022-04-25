namespace MechEngineer.Features.ArmorMaximizer;

public class ArmorMaximizerSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Allows to set max armor with proper ratios.";

	public bool HeadPointsUnChanged { get; set; } = true;
	public float CenterTorsoRatio { get; set; } = 0.73f;
	public float LeftTorsoRatio { get; set; } = 0.69f;
	public float RightTorsoRatio { get; set; } = 0.69f;

	public float H_Multiplier { get; set; } = 0.0776f;
	public float CT_Multiplier { get; set; } = 0.2000000f;
	public float LT_Multiplier { get; set; } = 0.145f;
	public float RT_Multiplier { get; set; } = 0.145f;
	public float LA_Multiplier { get; set; } = 0.110f;
	public float RA_Multiplier { get; set; } = 0.110f;
	public float LL_Multiplier { get; set; } = 0.145f;
	public float RL_Multiplier { get; set; } = 0.145f;
}