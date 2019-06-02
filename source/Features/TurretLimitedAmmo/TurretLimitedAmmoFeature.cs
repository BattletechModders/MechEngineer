namespace MechEngineer.Features.TurretLimitedAmmo
{
    internal class TurretLimitedAmmoFeature : Feature<BaseSettings>
    {
        internal static TurretLimitedAmmoFeature Shared = new TurretLimitedAmmoFeature();

        internal override BaseSettings Settings => Control.settings.TurretLimitedAmmo;
    }
}