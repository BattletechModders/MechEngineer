﻿namespace MechEngineer.Features.TurretMechComponents
{
    internal class TurretMechComponentsFeature : Feature<TurretMechComponentSettings>
    {
        internal static TurretMechComponentsFeature Shared = new();

        internal override TurretMechComponentSettings Settings => Control.settings.TurretMechComponents;
    }
}