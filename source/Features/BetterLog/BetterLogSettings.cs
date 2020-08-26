using HBS.Logging;

namespace MechEngineer.Features.BetterLog
{
    public class BetterLogSettings
    {
        public bool Enabled = true;

        public LogLevel Level = LogLevel.Log;
        public string LevelDescription => "The log level that will be logged, debug will tax the performance at some places and fill the logfile considerably.";

        public BetterLogFormatterSettings Formatter = new BetterLogFormatterSettings();
    }
}
