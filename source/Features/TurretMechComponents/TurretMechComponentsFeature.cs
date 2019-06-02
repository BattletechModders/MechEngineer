namespace MechEngineer.Features.TurretMechComponents
{
    internal class TurretMechComponentsFeature : Feature
    {
        internal static TurretMechComponentsFeature Shared = new TurretMechComponentsFeature();

        internal override bool Enabled => settings?.Enabled ?? false;

        internal static Settings settings => Control.settings.TurretMechComponents;

        public class Settings
        {
            public bool Enabled = true;
        }
    }
}