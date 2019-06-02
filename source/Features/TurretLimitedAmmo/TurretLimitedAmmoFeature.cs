namespace MechEngineer.Features.TurretLimitedAmmo
{
    internal class TurretLimitedAmmoFeature : Feature
    {
        internal static TurretLimitedAmmoFeature Shared = new TurretLimitedAmmoFeature();

        internal override bool Enabled => settings?.Enabled ?? false;

        internal static Settings settings => Control.settings.TurretLimitedAmmo;

        public class Settings
        {
            public bool Enabled = false;
        }
    }
}