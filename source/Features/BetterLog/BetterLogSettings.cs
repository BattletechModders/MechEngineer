namespace MechEngineer.Features.BetterLog;

public class BetterLogSettings : ISettings
{
    public bool Enabled => true;
    public string EnabledDescription => "Internal feature, always enabled.";

    public bool TraceEnabled = false;
    public string TraceEnabledDescription => "Enables additional debug logging that will fill the logfile even faster.";
}