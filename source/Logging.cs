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

    static Logging()
    {
        RefreshLogLevel();
        TrackLoggerLevelChanges();
    }

    private static readonly ILog LOG = Logger.GetLogger(nameof(MechEngineer), LogLevel.Debug);
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
        SyncLevelLogger(LogLevel.Error, ref Error);
        SyncLevelLogger(LogLevel.Warning, ref Warning);
        SyncLevelLogger(LogLevel.Log, ref Info);
        SyncLevelLogger(LogLevel.Debug, ref Debug);
        SyncLevelLogger((LogLevel)200, ref Trace);
    }

    private static void SyncLevelLogger(LogLevel logLevel, ref LevelLogger? field)
    {
        var log = (Logger.LogImpl)LOG;
        if (log.IsEnabledFor(logLevel))
        {
            field ??= new(LOG, logLevel);
        }
        else
        {
            field = null;
        }
    }

    internal sealed class LevelLogger
    {
        private readonly ILog log;
        private readonly LogLevel level;

        internal LevelLogger(ILog log, LogLevel level)
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

        public void Log(Exception e)
        {
            log.LogAtLevel(level, null, e);
        }
    }
}