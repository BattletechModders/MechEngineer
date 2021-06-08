using System;
using System.IO;
using HBS.Logging;
using Object = UnityEngine.Object;

namespace MechEngineer.Features.BetterLog
{
    internal sealed class BetterLog : ILogAppender, IDisposable
    {
        private readonly StreamWriter streamWriter;
        private readonly BetterLogFormatter formatter;

        private BetterLog(string path, BetterLogSettings settings)
        {
            streamWriter = new StreamWriter(path) { AutoFlush = true };
            formatter = new BetterLogFormatter(settings.Formatter);
        }

        public void OnLogMessage(string logName, LogLevel level, object message, Object context, Exception exception, IStackTrace location)
        {
            var formatted = formatter.GetFormattedLogLine(logName, level, message, context, exception, location);
            streamWriter.WriteLine(formatted);
        }

        public void Dispose()
        {
            streamWriter?.Dispose();
        }

        internal static BetterLogger SetupModLog(string path, string name, BetterLogSettings settings)
        {
            if (!settings.Enabled)
            {
                return new BetterLogger(null, LogLevel.Error);
            }

            var log = Logger.GetLogger(name);
            var appender = new BetterLog(path, settings);
            Logger.AddAppender(name, appender);
            Logger.SetLoggerLevel(name, settings.Level);
            var logger = new BetterLogger(log, settings.Level);
            return logger;
        }

        public void Flush()
        {
            streamWriter.Flush();
        }
    }
}
