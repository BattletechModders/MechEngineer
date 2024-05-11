namespace MechEngineer.Features.ComponentExplosions;

public class ComponentExplosionsSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Allows each component to define destructive forces in case they explode, also implements proper CASE.";

    public bool PassthroughHeadToCenterTorso { get; set; }
    public string PassthroughHeadToCenterTorsoorsoDescription => "Pass excessive head damage to the center torso.";
}