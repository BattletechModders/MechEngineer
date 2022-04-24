namespace MechEngineer.Features.ArmorStructureChanges;

public class ArmorStructureChangesSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Enables statistic effects for multiplying structure and armor values, happens before/after combat.";
}