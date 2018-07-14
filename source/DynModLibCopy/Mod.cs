using System;
using System.IO;
using HBS.Util;
using BattleTech;
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
        }

        public string Name { get; }
        public string Directory { get; }

        public string SourcePath => Path.Combine(Directory, "source");
        public string SettingsPath => Path.Combine(Directory, "Settings.json");
        public string DefaultsSettingsPath => Path.Combine(Directory, "Settings.defaults.json");
        public string ModsPath => Path.GetDirectoryName(Directory);
        public string InfoPath => Path.Combine(Directory, "mod.json");

        public ILog Logger => HBS.Logging.Logger.GetLogger(Name);
        private FileLogAppender logAppender;

        public void LoadSettings<T>(T settings) where T : ModSettings
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

            var logLevelString = settings.logLevel;
            DebugBridge.StringToLogLevel(logLevelString, out var level);
            if (level == null)
            {
                level = LogLevel.Debug;
            }
            HBS.Logging.Logger.SetLoggerLevel(Name, level);
        }

        public void SaveSettings<T>(T settings, string path) where T : ModSettings
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

        internal void SetupLogging()
        {
            var logFilePath = Path.Combine(Directory, "log.txt");
            try
            {
                ShutdownLogging();
                AddLogFileForLogger(Name, logFilePath);
            }
            catch (Exception e)
            {
                Logger.Log("DynModLib: can't create log file", e);
            }
        }

        internal void ShutdownLogging()
        {
            if (logAppender == null)
            {
                return;
            }

            try
            {
                HBS.Logging.Logger.ClearAppender(Name);
                logAppender.Flush();
                logAppender.Close();
            }
            catch
            {
            }

            logAppender = null;
        }

        private void AddLogFileForLogger(string name, string logFilePath)
        {
            logAppender = new FileLogAppender(logFilePath, FileLogAppender.WriteMode.INSTANT);

            HBS.Logging.Logger.AddAppender(name, logAppender);
        }

        public override string ToString()
        {
            return $"{Name} ({Directory})";
        }
    }
}
