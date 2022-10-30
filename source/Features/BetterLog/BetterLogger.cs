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
        SyncLevelLogger(level > LogLevel.Warning, LogLevel.Warning, ref Warning);
        SyncLevelLogger(level > LogLevel.Log, LogLevel.Log, ref Info);
        SyncLevelLogger(level > LogLevel.Debug, LogLevel.Debug, ref Debug);
        SyncLevelLogger(level > LogLevel.Debug || !traceEnabled, LogLevel.Debug, ref Trace);
    }

    private void SyncLevelLogger(bool disabled, LogLevel logLevel, ref BetterLevelLogger? field)
    {
        if (disabled)
        {
            field = null;
        }
        else if (field == null)
        {
            field = new BetterLevelLogger(log, logLevel);
        }
    }
}