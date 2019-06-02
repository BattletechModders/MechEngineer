namespace MechEngineer.Features.ShutdownInjuryProtection
{
    internal class ShutdownInjuryProtectionFeature : Feature
    {
        internal static ShutdownInjuryProtectionFeature Shared = new ShutdownInjuryProtectionFeature();

        internal override bool Enabled => (settings?.Enabled ?? false) && (settings.ShutdownInjuryEnabled || settings.HeatDamageInjuryEnabled);

        internal static Settings settings => Control.settings.ShutdownInjuryProtection;

        public class Settings
        {
            public bool Enabled = true;
            public bool HeatDamageInjuryEnabled = true;
            public bool ShutdownInjuryEnabled = true;
        }
    }
}