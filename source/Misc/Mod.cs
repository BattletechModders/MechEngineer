using System.IO;
using fastJSON;
using HBS.Util;

namespace MechEngineer.Misc;

internal class Mod
{
    internal Mod(string directory)
    {
        Directory = directory;
        Name = Path.GetFileName(directory);
    }

    private string Name { get; }
    public string Directory { get; }

    private string SettingsPath => Path.Combine(Directory, "Settings.json");
    public string SettingsDefaultsPath => Path.Combine(Directory, "Settings.defaults.json");
    public string SettingsHelpPath => Path.Combine(Directory, "Settings.help.json");
    public string SettingsLastPath => Path.Combine(Directory, "Settings.last.json");

    internal void LoadSettings(object settings)
    {
        if (!File.Exists(SettingsPath))
        {
            return;
        }

        using var reader = new StreamReader(SettingsPath);
        var json = reader.ReadToEnd();
        JSONSerializationUtility.FromJSON(settings, json);
    }

    internal void SaveSettings(object settings, string path)
    {
        using var writer = new StreamWriter(path);
        var p = new JSONParameters
        {
            EnableAnonymousTypes = true,
            SerializeToLowerCaseNames = false,
            UseFastGuid = false,
            KVStyleStringDictionary = false,
            SerializeNullValues = true
        };

        var json = JSON.ToNiceJSON(settings, p);
        writer.Write(json);
    }

    public override string ToString()
    {
        return $"{Name} ({Directory})";
    }
}