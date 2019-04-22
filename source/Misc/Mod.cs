using System;
using System.Diagnostics;
using System.IO;
using fastJSON;
using HBS.Logging;
using HBS.Util;

namespace MechEngineer
{
    public class Mod
    {
        public Mod(string directory)
        {
            Directory = directory;
            Name = Path.GetFileName(directory);
            Logger = HBS.Logging.Logger.GetLogger(Name);
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
}
