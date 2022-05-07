using HBS.Logging;

namespace MechEngineer.Features.BetterLog;

internal class BetterLogger
{
    public readonly BetterLevelLogger Error;
    public BetterLevelLogger? Warning;
    public BetterLevelLogger? Info;
    public BetterLevelLogger? Debug;
    public BetterLevelLogger? Trace;

    public string Name => log.Name;

    private readonly ILog log;
    private readonly bool traceEnabled;

    internal BetterLogger(ILog log, bool traceEnabled)
    {
        this.log = log;
        this.traceEnabled = traceEnabled;
        Error = new BetterLevelLogger(this.log, LogLevel.Error);
        RefreshLogLevel();
    }

    internal void RefreshLogLevel()
    {
        Logger.GetLoggerLevel(log.Name, out var level);
        Warning = level > LogLevel.Warning ? null : new BetterLevelLogger(log, LogLevel.Warning);
        Info = level > LogLevel.Log ? null : new BetterLevelLogger(log, LogLevel.Log);
        Debug = level > LogLevel.Debug ? null : new BetterLevelLogger(log, LogLevel.Debug);
        Trace = level > LogLevel.Debug || !traceEnabled ? null : new BetterLevelLogger(log, LogLevel.Debug);
    }
}