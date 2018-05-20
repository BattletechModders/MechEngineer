
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
        public static Mod mod;

        public static MechEngineModSettings settings = new MechEngineModSettings();
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

        // main engine + engine slots
        internal static bool IsEnginePart(this MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, "emod_engine");
        }

        // only main engine
        internal static bool IsMainEngine(this MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, "emod_engine_");
        }

        // we want to know about center torso upgrade (gyros), since we reduce their size
        internal static bool IsCenterTorsoUpgrade(this MechComponentDef componentDef)
        {
            return componentDef.AllowedLocations == ChassisLocations.CenterTorso && componentDef.ComponentType == ComponentType.Upgrade;
        }

        // endo steel has some calculations behind it
        internal static bool IsEndoSteel(this MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, "emod_structureslots_endosteel");
        }

        private static bool CheckComponentDef(MechComponentDef componentDef, ComponentType type, string prefix)
        {
            if (componentDef.ComponentType != type)
            {
                return false;
            }

            if (componentDef == null || componentDef.Description == null || componentDef.Description.Id == null)
            {
                return false;
            }

            return componentDef.Description.Id.StartsWith(prefix);
        }

        internal static bool IsDouble(this HeatSinkDef def)
        {
            return def.Description.Id == "Gear_HeatSink_Generic_Double";
        }

        internal static bool IsSingle(this HeatSinkDef def)
        {
            return def.Description.Id == "Gear_HeatSink_Generic_Standard";
        }
    }
}
