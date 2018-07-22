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
        public string LogPath => Path.Combine(Directory, "log.txt");

        public ILog Logger => LogManager.GetLogger(Name);

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
