using System;
using Harmony;
using HBS.Logging;
using LogLevel = HBS.Logging.LogLevel;

namespace MechEngineer;

internal static class Logging
{
    internal static LevelLogger? Error;
    internal static LevelLogger? Warning;
    internal static LevelLogger? Info;
    internal static LevelLogger? Debug;
    internal static LevelLogger? Trace;

    private static readonly ILog LOG = Logger.GetLogger(nameof(MechEngineer), LogLevel.Debug);

    static Logging()
    {
        RefreshLogLevel();
    }

    private static bool _traceEnabled = true;
    internal static void Setup(bool traceEnabled)
    {
        _traceEnabled = traceEnabled;
        RefreshLogLevel();
        TrackLoggerLevelChanges();
    }

    private static void TrackLoggerLevelChanges()
    {
        HarmonyInstance
            .Create(typeof(Logging).FullName)
            .Patch(
                original: typeof(Logger).GetMethod(nameof(Logger.SetLoggerLevel)),
                postfix: new(typeof(Logging), nameof(Logger_SetLoggerLevel_Postfix))
            );
    }
    private static void Logger_SetLoggerLevel_Postfix(string name)
    {
        try
        {
            if (name == LOG.Name)
            {
                RefreshLogLevel();
            }
        }
        catch (Exception e)
        {
            Error?.Log(e);
        }
    }

    private static void RefreshLogLevel()
    {
        Logger.GetLoggerLevel(LOG.Name, out var level);
        SyncLevelLogger(level > LogLevel.Error, LogLevel.Error, ref Error);
        SyncLevelLogger(level > LogLevel.Warning, LogLevel.Warning, ref Warning);
        SyncLevelLogger(level > LogLevel.Log, LogLevel.Log, ref Info);
        SyncLevelLogger(level > LogLevel.Debug, LogLevel.Debug, ref Debug);
        SyncLevelLogger(level > LogLevel.Debug || !_traceEnabled, LogLevel.Debug, ref Trace);
    }

    private static void SyncLevelLogger(bool disabled, LogLevel logLevel, ref LevelLogger? field)
    {
        if (disabled)
        {
            field = null;
        }
        else if (field == null)
        {
            field = new(LOG, logLevel);
        }
    }

    internal class LevelLogger
    {
        private readonly ILog log;
        private readonly LogLevel level;

        public LevelLogger(ILog log, LogLevel level)
        {
            this.log = log;
            this.level = level;
        }

        public void Log(object message)
        {
            log.LogAtLevel(level, message);
        }

        public void Log(object message, Exception e)
        {
            log.LogAtLevel(level, message, e);
        }

        public void Log(Func<string> callback)
        {
            log.LogAtLevel(level, callback());
        }
    }
}