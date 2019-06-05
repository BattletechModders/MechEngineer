namespace MechEngineer.Features.DebugCycleCombatSounds
{
    public class DebugCycleCombatSoundsSettings : ISettings
    {
        public bool Enabled { get; set; } = false;
        public string EnabledDescription => "Cycle through SFX sounds when pressing a specific button on the start menu";

        public string SpecificButton = "Skirmish";
    }
}