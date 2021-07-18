using HBS.Logging;

namespace MechEngineer.Features.BetterLog
{
    public class BetterLogSettings
    {
        public bool Enabled = true;

        public LogLevel Level = LogLevel.Debug;
        public string LevelDescription => "The log level that will be logged, debug will tax the performance at some places and fill the logfile considerably.";

        public bool TraceEnabled = false;
        public string TraceEnabledDescription => "Enables additional debug logging that will fill the logfile even faster.";
    }
}