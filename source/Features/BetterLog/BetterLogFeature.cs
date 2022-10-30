using System.Collections.Generic;
using System.Linq;
using HBS.Logging;

namespace MechEngineer.Features.BetterLog;

internal class BetterLogFeature : Feature<BetterLogSettings>
{
    internal static readonly BetterLogFeature Shared = new();

    internal override BetterLogSettings Settings => Control.Settings.BetterLog;

    private static readonly List<BetterLogger> Loggers = new();

    internal static BetterLogger SetupLog(string name, BetterLogSettings settings)
    {
        var log = Logger.GetLogger(name, LogLevel.Debug);
        var logger = new BetterLogger(log, settings.TraceEnabled);
        Loggers.Add(logger);
        return logger;
    }

    internal static void OnSetLoggerLevel(string loggerName)
    {
        foreach (var logger in Loggers.Where(l => l.Name == loggerName))
        {
            logger.RefreshLogLevel();
        }
    }
}