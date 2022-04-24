namespace MechEngineer.Features.DamageIgnore;

public class DamageIgnoreSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Enables ignore_damage flag.";
}