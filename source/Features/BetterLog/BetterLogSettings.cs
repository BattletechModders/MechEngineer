using HBS.Logging;

namespace MechEngineer.Features.BetterLog
{
    public class BetterLogSettings
    {
        public bool Enabled = false;
        public LogLevel Level = LogLevel.Debug;

        public BetterLogFormatterSettings Formatter = new BetterLogFormatterSettings();
    }
}
