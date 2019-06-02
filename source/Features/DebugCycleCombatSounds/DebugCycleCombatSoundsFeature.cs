namespace MechEngineer.Features.DebugCycleCombatSounds
{
    internal class DebugCycleCombatSoundsFeature : Feature
    {
        internal static DebugCycleCombatSoundsFeature Shared = new DebugCycleCombatSoundsFeature();

        internal override bool Enabled => settings?.Enabled ?? false;

        internal static Settings settings => Control.settings.DebugCycleCombatSounds;

        public class Settings
        {
            public bool Enabled = false;
        }
    }
}
