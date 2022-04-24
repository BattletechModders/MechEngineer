namespace MechEngineer.Features.Globals;

public class GlobalsSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Allows tooltips and other features to access the current mech from context.";
}