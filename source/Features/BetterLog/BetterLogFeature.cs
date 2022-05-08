using System.Collections.Generic;
using System.Linq;
using BattleTech;
using HBS.Logging;

namespace MechEngineer.Features.BetterLog;

internal class BetterLogFeature : Feature<BetterLogSettings>
{
    internal static readonly BetterLogFeature Shared = new();

    internal override BetterLogSettings Settings => Control.Settings.BetterLog;

    private static readonly List<BetterLogger> Loggers = new();

    internal static BetterLogger SetupLog(string path, string name, BetterLogSettings settings)
    {
        var log = Logger.GetLogger(name, LogLevel.Debug);
        var appender = new BetterLogAppender(path);
        Logger.AddAppender(name, appender);
        // ModTek first loads mod DLLs, then merges json, then reloads debug settings
        // that is too late for us
        // Logger.SetLoggerLevel(log.Name, GetConfiguredLogLevel(log.Name));
        var logger = new BetterLogger(log, settings.TraceEnabled);
        Loggers.Add(logger);
        return logger;
    }

    private static LogLevel? GetConfiguredLogLevel(string loggerName)
    {
        if (DebugBridge.settings.loggerLevels.TryGetValue(loggerName, out var level))
        {
            return level;
        }
        if (DebugBridge.settings.loggerLevels.TryGetValue("*", out level))
        {
            return level;
        }
        return null;
    }

    internal static void OnSetLoggerLevel(string loggerName)
    {
        foreach (var logger in Loggers.Where(l => l.Name == loggerName))
        {
            logger.RefreshLogLevel();
        }
    }
}