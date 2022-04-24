namespace MechEngineer.Features.MoveMultiplierStat;

public class MoveMultiplierStatSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Introduces a new statistic effect to allow easy manipulation of (mech only) movement.";
}