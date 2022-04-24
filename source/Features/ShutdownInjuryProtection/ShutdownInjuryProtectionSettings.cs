namespace MechEngineer.Features.ShutdownInjuryProtection;

public class ShutdownInjuryProtectionSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Pilots receive injury when mech receives overheat damage or has to shut down in case life support is destroyed.";

    public bool HeatDamageInjuryEnabled = true;
    public string HeatDamageInjuryEnabledDescription => "Every time the mech received heat damage, check if an injury can be applied.";

    public bool OverheatedOnActivationEndInjuryEnabled = true;
    public string OverheatedOnActivationEndInjuryDescription => "Every time a mech is overheated at the end of its round, check if an injury can be applied.";

    public bool ShutdownInjuryEnabled = true;
    public string ShutdownInjuryEnabledDescription => "Every time a mech shuts down, check if an injury can be applied.";
}