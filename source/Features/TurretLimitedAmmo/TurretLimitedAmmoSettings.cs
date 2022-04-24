namespace MechEngineer.Features.TurretLimitedAmmo;

public class TurretLimitedAmmoSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Turrets use up ammo and don't explode once ammo is gone.";
}