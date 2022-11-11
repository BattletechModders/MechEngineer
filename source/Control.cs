using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BattleTech;
using MechEngineer.Features;
using MechEngineer.Misc;

namespace MechEngineer;

public static class Control
{
    internal static Mod Mod = null!;

    internal static readonly MechEngineerSettings Settings = new();

    [UsedBy(User.ModTek)]
    public static void Start(string modDirectory, string json)
    {
        Mod = new(modDirectory);

        FileUtils.SetReadonly(Mod.SettingsDefaultsPath, false);
        File.Delete(Mod.SettingsDefaultsPath);

        FileUtils.SetReadonly(Mod.SettingsHelpPath, false);
        FileUtils.SetReadonly(Mod.SettingsLastPath, false);

        Mod.SaveSettings(Settings, Mod.SettingsHelpPath);
        Mod.LoadSettings(Settings);
        Mod.SaveSettings(Settings, Mod.SettingsLastPath);

        if (Settings.GeneratedSettingsFilesReadonly)
        {
            FileUtils.SetReadonly(Mod.SettingsHelpPath, true);
            FileUtils.SetReadonly(Mod.SettingsLastPath, true);
        }

        Logging.Setup(Settings.TraceEnabled);

        try
        {
            Logging.Info?.Log($"version {Assembly.GetExecutingAssembly().GetInformationalVersion()}");
            Logging.Info?.Log("settings loaded");
            Logging.Debug?.Log("debugging enabled");

            Logging.Debug?.Log("setting up features");

            foreach (var feature in FeaturesList.Features)
            {
                feature.SetupFeature();
            }

            Logging.Info?.Log("started");
        }
        catch (Exception e)
        {
            Logging.Error?.Log("error starting", e);
            throw;
        }
    }

    [UsedBy(User.ModTek)]
    public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
    {
        try
        {
            foreach (var feature in FeaturesList.Features)
            {
                feature.SetupFeatureResources(customResources);
            }

            Logging.Info?.Log("loaded");
        }
        catch (Exception e)
        {
            Logging.Error?.Log("error loading", e);
        }
    }

    private static string GetInformationalVersion(this Assembly assembly)
    {
        var attributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);

        if (attributes.Length == 0)
        {
            return "";
        }

        return ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
    }
}