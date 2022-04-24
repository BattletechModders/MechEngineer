namespace MechEngineer.Features.CompressFloatieMessages;

public class CompressFloatieMessagesSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Compresses similar floatie messages to a single message with a multiplier. E.g. MEDIUM LASER DESTROYED x 4";
}