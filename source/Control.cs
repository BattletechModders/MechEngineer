using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BattleTech;
using MechEngineer.Features;
using MechEngineer.Features.BetterLog;
using MechEngineer.Misc;

namespace MechEngineer
{
    public static class Control
    {
        internal static Mod mod;

        internal static BetterLogger Logger;

        internal static MechEngineerSettings settings = new MechEngineerSettings();

        public static void Start(string modDirectory, string json)
        {
            mod = new Mod(modDirectory);
            try
            {
                FileUtils.SetReadonly(mod.SettingsDefaultsPath, false);
                FileUtils.SetReadonly(mod.SettingsLastPath, false);

                mod.SaveSettings(settings, mod.SettingsDefaultsPath);
                mod.LoadSettings(settings);
                mod.SaveSettings(settings, mod.SettingsLastPath);

                if (settings.GeneratedSettingsFilesReadonly)
                {
                    FileUtils.SetReadonly(mod.SettingsDefaultsPath, true);
                    FileUtils.SetReadonly(mod.SettingsLastPath, true);
                }

                Logger = BetterLog.SetupModLog(Path.Combine(modDirectory, "log.txt"), nameof(MechEngineer), settings.BetterLog);
                Logger.Info.Log($"version {Assembly.GetExecutingAssembly().GetInformationalVersion()}");
                Logger.Info.Log("settings loaded");
                Logger.Debug?.Log("debugging enabled");

                Logger.Debug?.Log("setting up features");
                
                foreach (var feature in FeaturesList.Features)
                {
                    feature.SetupFeature();
                }

                Logger.Info.Log("started");
            }
            catch (Exception e)
            {
                Logger.Error.Log("error starting", e);
                throw;
            }
        }
        
        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            try
            {
                foreach (var feature in FeaturesList.Features)
                {
                    feature.SetupFeatureResources(customResources);
                }

                Logger.Info.Log("loaded");
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
}
