using System;
using System.Collections.Generic;
using System.Reflection;
using BattleTech;
using CustomComponents;
using Harmony;
using MechEngineer.Features;
using MechEngineer.Features.AutoFix;

namespace MechEngineer
{
    public static class Control
    {
        internal static Mod mod;

        internal static MechEngineerSettings settings = new MechEngineerSettings();
        internal static HarmonyInstance harmony;

        public static void Start(string modDirectory, string json)
        {
            mod = new Mod(modDirectory);
            try
            {
                mod.SaveSettings(settings, mod.SettingsDefaultsPath);
                mod.LoadSettings(settings);
                mod.SaveSettings(settings, mod.SettingsLastPath);

                mod.Logger.Log($"version {Assembly.GetExecutingAssembly().GetInformationalVersion()}");
                mod.Logger.Log("settings loaded");
                mod.Logger.LogDebug("debugging enabled");

                mod.Logger.LogDebug("setting up features");
                //HarmonyInstance.DEBUG = true;
                harmony = HarmonyInstance.Create(mod.Name);
                foreach (var feature in FeaturesList.Features)
                {
                    feature.SetupFeature();
                }
                harmony = null;

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
