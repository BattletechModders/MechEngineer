#nullable enable
using HBS.Logging;

namespace MechEngineer.Features.BetterLog
{
    internal class BetterLogger
    {
        public readonly BetterLevelLogger Error;
        public readonly BetterLevelLogger Warning;
        public readonly BetterLevelLogger Info;
        public readonly BetterLevelLogger? Debug;
        public readonly BetterLevelLogger? Trace;

        private BetterLogger(ILog log, LogLevel level, bool traceEnabled)
        {
            Error = new BetterLevelLogger(log, LogLevel.Error);
            Warning = new BetterLevelLogger(log, LogLevel.Warning);
            Info = new BetterLevelLogger(log, LogLevel.Log);
            if (level > LogLevel.Debug)
            {
                return;
            }
            Debug = new BetterLevelLogger(log, LogLevel.Debug);
            if (!traceEnabled)
            {
                return;
            }
            Trace = new BetterLevelLogger(log, LogLevel.Debug);
        }

        internal static BetterLogger SetupModLog(string path, string name, BetterLogSettings settings)
        {
            var log = Logger.GetLogger(name);
            if (!settings.Enabled)
            {
                return new BetterLogger(log, LogLevel.Error, false);
            }
            var appender = new BetterLogAppender(path);
            Logger.AddAppender(name, appender);
            Logger.SetLoggerLevel(name, settings.Level);
            var logger = new BetterLogger(log, settings.Level, settings.TraceEnabled);
            return logger;
        }
    }
}