namespace MechEngineer.Features.ShutdownInjuryProtection
{
    internal class ShutdownInjuryProtectionFeature : Feature<ShutdownInjuryProtectionSettings>
    {
        internal static ShutdownInjuryProtectionFeature Shared = new();

        internal override bool Enabled => base.Enabled && (settings.ShutdownInjuryEnabled || settings.HeatDamageInjuryEnabled);

        internal override ShutdownInjuryProtectionSettings Settings => Control.settings.ShutdownInjuryProtection;

        internal static ShutdownInjuryProtectionSettings settings => Shared.Settings;
    }
}