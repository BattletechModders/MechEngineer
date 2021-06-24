namespace MechEngineer.Features.DebugCycleCombatSounds
{
    internal class DebugCycleCombatSoundsFeature : Feature<DebugCycleCombatSoundsSettings>
    {
        internal static DebugCycleCombatSoundsFeature Shared = new();

        internal override DebugCycleCombatSoundsSettings Settings => Control.settings.DebugCycleCombatSounds;
    }
}