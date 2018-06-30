using System;
using System.Reflection;
using BattleTech;
using DynModLib;
using Harmony;

namespace MechEngineer
{
    public static class Control
    {
        internal static Mod mod;

        internal static MechEngineerSettings settings = new MechEngineerSettings();
        internal static EngineCalculator calc = new EngineCalculator();

        private static CombatGameConstants _combat;

        public static CombatGameConstants Combat
        {
            get { return _combat ?? (_combat = CombatGameConstants.CreateFromSaved(UnityGameInstance.BattleTechGame)); }
        }

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
    }
}