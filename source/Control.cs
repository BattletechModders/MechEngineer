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

                Registry.RegisterPreProcessor(CockpitHandler.Shared);
                Registry.RegisterPreProcessor(GyroHandler.Shared);
                Registry.RegisterPreProcessor(LegActuatorHandler.Shared);

                Validator.RegisterMechValidator(DynamicSlotHandler.Shared.CCValidation.ValidateMech, DynamicSlotHandler.Shared.CCValidation.ValidateMechCanBeFielded);
                if (settings.UseArmActuators)
                {
                    Validator.RegisterClearInventory(ArmActuatorHandler.ClearInventory);

                    if (settings.ForceFullDefaultActuators)
                    {
                        Validator.RegisterMechValidator(ArmActuatorHandler.ValidateMechFF,
                            ArmActuatorHandler.CanBeFieldedFF);

                        AutoFixer.Shared.RegisterMechFixer(ArmActuatorHandler.FixCBTActuatorsFF);
                    }
                    else
                    {
                        Validator.RegisterMechValidator(ArmActuatorHandler.ValidateMech,
                            ArmActuatorHandler.CanBeFielded);

                        AutoFixer.Shared.RegisterMechFixer(ArmActuatorHandler.FixCBTActuators);
                    }
                }

                if (settings.DynamicSlotsValidateDropEnabled)
                {
                    Validator.RegisterDropValidator(check: DynamicSlotHandler.Shared.CCValidation.ValidateDrop);
                }

                Validator.RegisterMechValidator(EngineValidation.Shared.CCValidation.ValidateMech, EngineValidation.Shared.CCValidation.ValidateMechCanBeFielded);

                //Validator.RegisterMechValidator(ArmActuatorHandler.Shared.CCValidation.ValidateMech, ArmActuatorHandler.Shared.CCValidation.ValidateMechCanBeFielded);
                //Validator.RegisterDropValidator(check: ArmActuatorHandler.Shared.CCValidation.ValidateDrop);

                mod.Logger.LogDebug("setting up mechdef auto fixers");

                AutoFixer.Shared.RegisterMechFixer(MEAutoFixer.Shared.AutoFix);

                // logging output can be found under BATTLETECH\BattleTech_Data\output_log.txt
                // or also under yourmod/log.txt
                mod.Logger.Log("started");
            }
            catch (Exception e)
            {
                mod.Logger.LogError("error starting", e);
            }
        }

        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            try
            {
                BonusDescriptions.Setup(customResources);
                CriticalEffectsHandler.Setup(customResources);

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
