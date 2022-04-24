namespace MechEngineer.Features.DebugCycleCombatSounds;

public class DebugCycleCombatSoundsSettings : ISettings
{
    public bool Enabled { get; set; } = false;
    public string EnabledDescription => "Cycle through SFX sounds when pressing a specific button on the start menu";

    public bool DebugMainCycleSoundsOnReceiveButtonEnabled { get; } = false;
    public string DebugMainMenuReceiveButtonCycleSoundsDescription => $"Cycle sounds and play them when pressing the button defined under {nameof(SpecificButton)}";

    public string SpecificButton = "Skirmish";
}