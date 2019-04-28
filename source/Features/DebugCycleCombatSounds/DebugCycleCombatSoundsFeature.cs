namespace MechEngineer.Features.DebugCycleCombatSounds
{
    internal class DebugCycleCombatSoundsFeature : Feature
    {
        internal static DebugCycleCombatSoundsFeature Shared = new DebugCycleCombatSoundsFeature();

        internal override bool Enabled => Control.settings.DebugCycleCombatSoundsFeatureEnabled;
    }
}
