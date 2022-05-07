namespace MechEngineer.Features.TurretMechComponents;

internal class TurretMechComponentsFeature : Feature<TurretMechComponentSettings>
{
    internal static readonly TurretMechComponentsFeature Shared = new();

    internal override TurretMechComponentSettings Settings => Control.Settings.TurretMechComponents;
}