namespace MechEngineer.Features.ShutdownInjuryProtection
{
    internal class ShutdownInjuryProtectionFeature : Feature
    {
        internal static ShutdownInjuryProtectionFeature Shared = new ShutdownInjuryProtectionFeature();

        internal override bool Enabled => Control.settings.ShutdownInjuryEnabled || Control.settings.HeatDamageInjuryEnabled;
    }
}