using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BattleTech;
using MechEngineer.Features;
using MechEngineer.Features.BetterLog;
using MechEngineer.Misc;

namespace MechEngineer;

public static class Control
{
    internal static Mod Mod = null!;
    internal static BetterLogger Logger = null!;

    internal static readonly MechEngineerSettings Settings = new();

    [UsedByModTek]
    public static void Start(string modDirectory, string json)
    {
        Mod = new(modDirectory);
        Mod.ResetStartupErrorLog();
        try
        {
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

            Logger = BetterLogFeature.SetupLog(Path.Combine(modDirectory, "log.txt"), nameof(MechEngineer), Settings.BetterLog);
        }
        catch (Exception e)
        {
            Mod.WriteStartupError(e);
            throw;
        }

        try
        {
            Logger.Info?.Log($"version {Assembly.GetExecutingAssembly().GetInformationalVersion()}");
            Logger.Info?.Log("settings loaded");
            Logger.Debug?.Log("debugging enabled");

            Logger.Debug?.Log("setting up features");

            foreach (var feature in FeaturesList.Features)
            {
                feature.SetupFeature();
            }

            Logger.Info?.Log("started");
        }
        catch (Exception e)
        {
            Logger.Error.Log("error starting", e);
            throw;
        }
    }

    [UsedByModTek]
    public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
    {
        try
        {
            foreach (var feature in FeaturesList.Features)
            {
                feature.SetupFeatureResources(customResources);
            }

            Logger.Info?.Log("loaded");
        }
        catch (Exception e)
        {
            Logger.Error.Log("error loading", e);
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