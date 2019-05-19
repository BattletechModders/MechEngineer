namespace MechEngineer.Features.TurretLimitedAmmo
{
    internal class TurretLimitedAmmoFeature : Feature
    {
        internal static TurretLimitedAmmoFeature Shared = new TurretLimitedAmmoFeature();

        internal override bool Enabled => Control.settings.FeatureTurretLimitedAmmoEnabled;
    }
}