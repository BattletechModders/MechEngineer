using System;
using System.IO;
using HBS.Logging;
using Object = UnityEngine.Object;

namespace MechEngineer.Features.BetterLog
{
    internal sealed class BetterLogAppender : ILogAppender, IDisposable
    {
        private readonly StreamWriter writer;
        private readonly BetterLogFormatter formatter = new();

        internal BetterLogAppender(string path)
        {
            writer = new StreamWriter(path) {AutoFlush = true};
            formatter = new BetterLogFormatter();
        }

        public void OnLogMessage(string logName, LogLevel level, object message, Object context, Exception exception, IStackTrace location)
        {
            var formatted = BetterLogFormatter.GetFormattedLogLine(level, message, exception);
            writer.WriteLine(formatted);
        }

        public void Dispose()
        {
            writer?.Dispose();
        }

        public void Flush()
        {
            writer.Flush();
        }
    }
}