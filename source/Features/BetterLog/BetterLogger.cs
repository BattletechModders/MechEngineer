using HBS.Logging;

namespace MechEngineer.Features.BetterLog
{
    internal class BetterLogger
    {
        public readonly BetterLevelLogger Error;
        public readonly BetterLevelLogger Warning;
        public readonly BetterLevelLogger Info;
        public readonly BetterLevelLogger Debug; // C# 8.0: "BetterLevelLogger?"

        public BetterLogger(ILog log, LogLevel level)
        {
            // error, warning and info are always active and should never be unset
            Error = new BetterLevelLogger(log, LogLevel.Error);
            Warning = new BetterLevelLogger(log, LogLevel.Warning);
            Info = new BetterLevelLogger(log, LogLevel.Log);
            if (level > LogLevel.Debug)
            {
                return;
            }
            Debug = new BetterLevelLogger(log, LogLevel.Debug);
        }
    }
}