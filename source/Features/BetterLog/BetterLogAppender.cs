using System;
using System.IO;
using HBS.Logging;
using LogLevel = HBS.Logging.LogLevel;
using Object = UnityEngine.Object;

namespace MechEngineer.Features.BetterLog;

internal sealed class BetterLogAppender : ILogAppender, IDisposable
{
    private readonly StreamWriter writer;

    internal BetterLogAppender(string path)
    {
        writer = new StreamWriter(path) {AutoFlush = true};
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