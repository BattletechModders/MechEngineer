
using System;
using HBS.Logging;
using Harmony;
using System.Reflection;
using BattleTech;
using DynModLib;
using Logger = HBS.Logging.Logger;

namespace MechEngineMod
{
    public static class Control
    {
        internal static Mod mod;

        internal static MechEngineModSettings settings = new MechEngineModSettings();
        internal static EngineCalculator calc = new EngineCalculator();

        public static void Start(string modDirectory, string json)
        {
            mod = new Mod(modDirectory);
            try
            {
                mod.LoadSettings(settings);
                
                var harmony = HarmonyInstance.Create(mod.Name);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                // logging output can be found under BATTLETECH\BattleTech_Data\output_log.txt
                // or also under yourmod/log.txt
                mod.Logger.Log("Loaded " + mod.Name);
            }
            catch (Exception e)
            {
                mod.Logger.LogError(e);
            }
        }

        private static CombatGameConstants _combat;

        public static CombatGameConstants Combat
        {
            get { return _combat ?? (_combat = CombatGameConstants.CreateFromSaved(UnityGameInstance.BattleTechGame)); }
        }

        // used id or id prefixes
        internal static string EnginePartPrefix = "emod_engine";
        internal static string MainEnginePrefix = "emod_engine_";
        internal static string EndoSteelPrefix = "emod_structureslots_endosteel";
        internal static string FerrosFibrousPrefix = "emod_structureslots_ferrosfibrous";
        internal static string GearHeatSinkGenericDouble = "Gear_HeatSink_Generic_Double";
        internal static string GearGyroGeneric = "Gear_Gyro_Generic_Standard";
        internal static string GearCockpitGeneric = "Gear_Cockpit_Generic_Standard";
        internal static string GearHeatSinkGenericStandard = "Gear_HeatSink_Generic_Standard";
        internal static string EngineKitDHS = "emod_kit_dhs";
    }
}
