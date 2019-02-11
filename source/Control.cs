using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BattleTech;
using CustomComponents;
using Harmony;

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
                mod.SaveSettings(settings, mod.SettingsDefaultsPath);
                mod.LoadSettings(settings);
                mod.SaveSettings(settings, mod.SettingsLastPath);

                LogManager.Setup(mod.LogPath, settings.LogLevels.ToDictionary(x => x.Name, x => x.Level));

                mod.Logger.Log($"version {Assembly.GetExecutingAssembly().GetInformationalVersion()}");
                mod.Logger.Log("settings loaded");
                mod.Logger.LogDebug("debugging enabled");
                
                mod.Logger.LogDebug("patching game");

                //HarmonyInstance.DEBUG = true;
                var harmony = HarmonyInstance.Create(mod.Name);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                //harmony.Patch(typeof(MechLabPanelLoadMechPatch));
                
                mod.Logger.LogDebug("setting up CustomComponents");
                Registry.RegisterSimpleCustomComponents(typeof(BonusDescriptions));
                Registry.RegisterSimpleCustomComponents(typeof(Weights));
                Registry.RegisterSimpleCustomComponents(typeof(EngineCoreDef));
                Registry.RegisterSimpleCustomComponents(Assembly.GetExecutingAssembly());
                
                Registry.RegisterPreProcessor(ArmActuatorHandler.Shared);
                Registry.RegisterPreProcessor(CockpitHandler.Shared);
                Registry.RegisterPreProcessor(GyroHandler.Shared);
                Registry.RegisterPreProcessor(LegActuatorHandler.Shared);

                Validator.RegisterMechValidator(DynamicSlotHandler.Shared.CCValidation.ValidateMech, DynamicSlotHandler.Shared.CCValidation.ValidateMechCanBeFielded);
                if (settings.DynamicSlotsValidateDropEnabled)
                {
                    Validator.RegisterDropValidator(check: DynamicSlotHandler.Shared.CCValidation.ValidateDrop);
                }

                Validator.RegisterMechValidator(EngineValidation.Shared.CCValidation.ValidateMech, EngineValidation.Shared.CCValidation.ValidateMechCanBeFielded);

                //Validator.RegisterMechValidator(ArmActuatorHandler.Shared.CCValidation.ValidateMech, ArmActuatorHandler.Shared.CCValidation.ValidateMechCanBeFielded);
                //Validator.RegisterDropValidator(check: ArmActuatorHandler.Shared.CCValidation.ValidateDrop);
                
                mod.Logger.LogDebug("setting up mechdef auto fixers");

                AutoFixer.Shared.RegisterMechFixer(MEAutoFixer.Shared.AutoFix);

                mod.Logger.LogDebug("added backwards compatibility assembly resolver");
                AllowAnyAssemblyVersionReference();
                
                // logging output can be found under BATTLETECH\BattleTech_Data\output_log.txt
                // or also under yourmod/log.txt
                mod.Logger.Log("loaded " + mod.Name);
            }
            catch (Exception e)
            {
                mod.Logger.LogError(e);
            }
        }

        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            BonusDescriptions.Setup(customResources);
        }

        private static string GetInformationalVersion(this Assembly assembly) {
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);

            if (attributes.Length == 0)
            {
                return "";
            }

            return ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
        }

        private static void AllowAnyAssemblyVersionReference()
        {
            // this code makes sure that dlls that depend on ME will always try to load even if the ME version is different than whats expected
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var resolvingName = new AssemblyName(args.Name);
                var assembly = typeof(Control).Assembly;
                if (resolvingName.Name != assembly.GetName().Name)
                {
                    return null;
                }

                mod.Logger.Log($"assembly resolve \"{args.Name}\" with \"{assembly.FullName}\"");
                return assembly;
            };
        }
    }
}
