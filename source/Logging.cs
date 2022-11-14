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

    private static readonly ILog s_log = Logger.GetLogger(nameof(MechEngineer), LogLevel.Debug);
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
            if (name == s_log.Name)
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
        var log = (Logger.LogImpl)s_log;
        if (log.IsEnabledFor(logLevel))
        {
            field ??= new(logLevel);
        }
        else
        {
            field = null;
        }
    }

    internal sealed class LevelLogger
    {
        private readonly LogLevel _level;

        internal LevelLogger(LogLevel level)
        {
            _level = level;
        }

        public void Log(object message)
        {
            s_log.LogAtLevel(_level, message);
        }

        public void Log(object message, Exception e)
        {
            s_log.LogAtLevel(_level, message, e);
        }

        public void Log(Exception e)
        {
            s_log.LogAtLevel(_level, null, e);
        }
    }
}