namespace MechEngineer.Features.DebugCycleCombatSounds
{
    internal class DebugCycleCombatSoundsFeature : Feature<BaseSettings>
    {
        internal static DebugCycleCombatSoundsFeature Shared = new DebugCycleCombatSoundsFeature();

        internal override BaseSettings Settings => Control.settings.DebugCycleCombatSounds;
    }
}
