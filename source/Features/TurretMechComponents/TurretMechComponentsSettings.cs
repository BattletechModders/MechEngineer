namespace MechEngineer.Features.TurretMechComponents;

public class TurretMechComponentSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Turrets can now use components and benefit from their status effects.";
}