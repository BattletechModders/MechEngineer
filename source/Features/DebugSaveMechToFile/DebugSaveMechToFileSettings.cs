namespace MechEngineer.Features.DebugSaveMechToFile;

public class DebugSaveMechToFileSettings : ISettings
{
    public bool Enabled { get; set; } = false;
    public string EnabledDescription => "Saves mechdefs to 'MechEngineer/Saves' when saving a mech in the mechlab";
}