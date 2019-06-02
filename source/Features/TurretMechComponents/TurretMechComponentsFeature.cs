namespace MechEngineer.Features.TurretMechComponents
{
    internal class TurretMechComponentsFeature : Feature<BaseSettings>
    {
        internal static TurretMechComponentsFeature Shared = new TurretMechComponentsFeature();

        internal override BaseSettings Settings => Control.settings.TurretMechComponents;
    }
}