using System;
using System.Linq;
using System.Reflection;
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

                mod.Logger.Log($"version {Assembly.GetExecutingAssembly().GetName().Version}");
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
                Registry.RegisterPreProcessor(LegUpgradeHandler.Shared);

                Validator.RegisterMechValidator(DynamicSlotHandler.Shared.CCValidation.ValidateMech, DynamicSlotHandler.Shared.CCValidation.ValidateMechCanBeFielded);
                if (settings.DynamicSlotsValidateDropEnabled)
                {
                    Validator.RegisterDropValidator(check: DynamicSlotHandler.Shared.CCValidation.ValidateDrop);
                }

                Validator.RegisterMechValidator(EngineValidation.Shared.CCValidation.ValidateMech, EngineValidation.Shared.CCValidation.ValidateMechCanBeFielded);

                Validator.RegisterMechValidator(ArmActuatorHandler.Shared.CCValidation.ValidateMech, ArmActuatorHandler.Shared.CCValidation.ValidateMechCanBeFielded);
                Validator.RegisterDropValidator(check: ArmActuatorHandler.Shared.CCValidation.ValidateDrop);

                foreach (var categoryDescriptor in settings.Categories)
                {
                    CustomComponents.Control.AddCategory(categoryDescriptor);
                }

                foreach (var restrictions in settings.TagRestrictions)
                {
                    CustomComponents.Control.AddTagRestrictions(restrictions);
                }
                
                mod.Logger.LogDebug("setting up mechdef auto fixers");

                MechDefAutoFixCategory.SetMechDefAutoFixCategory();
                
                // logging output can be found under BATTLETECH\BattleTech_Data\output_log.txt
                // or also under yourmod/log.txt
                mod.Logger.Log("Loaded " + mod.Name);
            }
            catch (Exception e)
            {
                mod.Logger.LogError(e);
            }
        }
    }
}