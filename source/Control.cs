using System;
using System.Linq;
using System.Reflection;
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
                mod.LoadSettings(settings);
                LogManager.Setup(mod.LogPath, settings.LogLevels.ToDictionary(x => x.Name, x => x.Level));
                
                mod.Logger.Log("settings loaded");
                mod.Logger.LogDebug("debugging enabled");

                mod.SaveSettings(settings, mod.DefaultsSettingsPath);

                
                mod.Logger.LogDebug("patching game");

                var harmony = HarmonyInstance.Create(mod.Name);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                //harmony.Patch(typeof(MechLabPanelLoadMechPatch));
                
                mod.Logger.LogDebug("setting up CustomComponents");

                CustomComponents.Registry.RegisterSimpleCustomComponents(Assembly.GetExecutingAssembly());
                
                CustomComponents.Registry.RegisterPreProcessor(ArmActuatorHandler.Shared);
                CustomComponents.Registry.RegisterPreProcessor(CockpitHandler.Shared);
                CustomComponents.Registry.RegisterPreProcessor(GyroHandler.Shared);
                CustomComponents.Registry.RegisterPreProcessor(LegUpgradeHandler.Shared);

                CustomComponents.Validator.RegisterMechValidator(DynamicSlotHandler.Shared.CCValidation.ValidateMech, DynamicSlotHandler.Shared.CCValidation.ValidateMechCanBeFielded);
                if (settings.DynamicSlotsValidateDropEnabled)
                {
                    CustomComponents.Validator.RegisterDropValidator(check: DynamicSlotHandler.Shared.CCValidation.ValidateDrop);
                }

                CustomComponents.Validator.RegisterMechValidator(EngineValidation.Shared.CCValidation.ValidateMech, EngineValidation.Shared.CCValidation.ValidateMechCanBeFielded);

                CustomComponents.Validator.RegisterMechValidator(ArmActuatorHandler.Shared.CCValidation.ValidateMech, ArmActuatorHandler.Shared.CCValidation.ValidateMechCanBeFielded);
                CustomComponents.Validator.RegisterDropValidator(check: ArmActuatorHandler.Shared.CCValidation.ValidateDrop);

                CustomComponents.Validator.RegisterMechValidator(TagRestrictionsHandler.Shared.CCValidation.ValidateMech, TagRestrictionsHandler.Shared.CCValidation.ValidateMechCanBeFielded);
                if (settings.TagRestrictionsValidateDropEnabled)
                {
                    CustomComponents.Validator.RegisterDropValidator(check: TagRestrictionsHandler.Shared.CCValidation.ValidateDrop);
                }

                foreach (var restriction in settings.TagRestrictions)
                {
                    TagRestrictionsHandler.Shared.Add(restriction);
                }

                foreach (var categoryDescriptor in settings.Categories)
                {
                    CustomComponents.Control.AddCategory(categoryDescriptor);
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