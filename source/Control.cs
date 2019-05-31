using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BattleTech;
using MechEngineer.Features;
using MechEngineer.Features.BetterLog;

namespace MechEngineer
{
    public static class Control
    {
        internal static Mod mod;

        internal static MechEngineerSettings settings = new MechEngineerSettings();

        public static void Start(string modDirectory, string json)
        {
            mod = new Mod(modDirectory);
            try
            {
                BetterLog.SetupModLog(Path.Combine(modDirectory, "log.txt"), nameof(MechEngineer), new BetterLogSettings { Enabled = true });

                mod.SaveSettings(settings, mod.SettingsDefaultsPath);
                mod.LoadSettings(settings);
                mod.SaveSettings(settings, mod.SettingsLastPath);

                mod.Logger.Log($"version {Assembly.GetExecutingAssembly().GetInformationalVersion()}");
                mod.Logger.Log("settings loaded");
                mod.Logger.LogDebug("debugging enabled");

                mod.Logger.LogDebug("setting up features");
                
                foreach (var feature in FeaturesList.Features)
                {
                    feature.SetupFeature();
                }

                mod.Logger.Log("started");
            }
            catch (Exception e)
            {
                mod.Logger.LogError("error starting", e);
                throw;
            }
        }
        
        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            try
            {
                foreach (var feature in FeaturesList.Features)
                {
                    feature.SetupResources(customResources);
                }

                mod.Logger.Log("loaded");
            }
            catch (Exception e)
            {
                mod.Logger.LogError("error loading", e);
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
