using System;
using System.Diagnostics;
using System.IO;
using HBS.Util;
using fastJSON;
using HBS.Logging;

namespace MechEngineer
{
    public class Mod
    {
        public Mod(string directory)
        {
            Directory = directory;
            Name = Path.GetFileName(directory);
            Logger = HBS.Logging.Logger.GetLogger(Name, LogLevel.Debug);
        }

        public string Name { get; }
        public string Directory { get; }
        public ILog Logger { get; }

        public string SourcePath => Path.Combine(Directory, "source");
        public string SettingsPath => Path.Combine(Directory, "Settings.json");
        public string SettingsDefaultsPath => Path.Combine(Directory, "Settings.defaults.json");
        public string SettingsLastPath => Path.Combine(Directory, "Settings.last.json");
        public string ModsPath => Path.GetDirectoryName(Directory);
        public string InfoPath => Path.Combine(Directory, "mod.json");
        public string LogPath => Path.Combine(Directory, "log.txt");

        public void SetupLogging(LogSettings settings)
        {
            if (settings.LogFileEnabled)
            {
                var appender = new FileLogAppender(LogPath, FileLogAppender.WriteMode.INSTANT);
                HBS.Logging.Logger.AddAppender(Logger.Name, appender);
            }
            HBS.Logging.Logger.SetLoggerLevel(Logger.Name, settings.LogLevel);
        }

        public void LoadSettings(object settings)
        {
            if (!File.Exists(SettingsPath))
            {
                return;
            }
            
            using (var reader = new StreamReader(SettingsPath))
            {
                var json = reader.ReadToEnd();
                JSONSerializationUtility.FromJSON(settings, json);
            }
        }

        public void SaveSettings(object settings, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                var p = new JSONParameters
                {
                    EnableAnonymousTypes = true,
                    SerializeToLowerCaseNames = false,
                    UseFastGuid = false,
                    KVStyleStringDictionary = false,
                    SerializeNullValues = false
                };

                var json = JSON.ToNiceJSON(settings, p);
                writer.Write(json);
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Directory})";
        }
    }
    
    public class LogSettings
    {
        public bool LogFileEnabled = true;
        public LogLevel LogLevel = LogLevel.Debug;
    }

    public static class LoggerExtensions
    {
        [Conditional("TRACE")]
        public static void Trace(this ILog logger, object message)
        {
            logger.LogDebug(message);
        }

        [Conditional("TRACE")]
        public static void Trace(this ILog logger, object message, Exception exception)
        {
            logger.LogDebug(message, exception);
        }
    }
}
