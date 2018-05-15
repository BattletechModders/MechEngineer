
using System;
using System.Collections.Generic;
using System.Linq;
using HBS.Logging;
using Harmony;
using System.Reflection;
using System.Text.RegularExpressions;
using BattleTech;
using BattleTech.UI;
using DynModLib;
using UnityEngine;
using Logger = HBS.Logging.Logger;

namespace MechEngineMod
{
    public class MechEngineModSettings : ModSettings
    {
        public int TechCostPerEngineTon = 1;
        public float? MaxSprintFactor = null;
        public float? MinSprintFactor = null;
    }

    internal class EngineCalculator
    {
        // r = t * w
        // s = w * 5/3
        // f = d * t / r
        // CDA-2A 350d 40t 320r // 44f
        // SHD-2H 240d 55t 275r // 48f
        // AS7-D 165d 100t 300r // 55f
        internal static float factorwalkMPtoSprintDistance = 50;

        internal static float MaxSprintFactor
        {
            get
            {
                //"MaxSprintFactor" : 350.0
                return Control.settings.MaxSprintFactor ?? UnityGameInstance.BattleTechGame.MechStatisticsConstants.MaxSprintFactor;
            }
        }

        internal static float MinSprintFactor
        {
            get
            {
                //"MinSprintFactor" : 125.0
                return Control.settings.MinSprintFactor ?? UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinSprintFactor;
            }
        }

        internal bool CalcAvailability(Engine engine, float tonnage)
        {
            Control.mod.Logger.LogDebug("CalcAvailability rating=" + engine.Rating + " tonnage=" + tonnage);
            //// MWO style max rating
            //if (engine.Rating > tonnage * 8.5)
            //{
            //    return false;
            //}

            var sprintDistance = DistanceFormula(engine.Rating, tonnage);

            Control.mod.Logger.LogDebug("CalcAvailability" +
                                        " rating=" + engine.Rating +
                                        " tonnage=" + tonnage +
                                        " sprintDistance=" + sprintDistance +
                                        " MaxSprintFactor=" + MaxSprintFactor +
                                        " MinSprintFactor=" + MinSprintFactor);

            // max rating due to sprint max
            if (sprintDistance > MaxSprintFactor)
            {
                return false;
            }

            // min rating due to sprint min
            if (sprintDistance < MinSprintFactor)
            {
                return false;
            }

            return true;
        }

        internal void CalcSpeeds(Engine engine, float tonnage, out float walkSpeed, out float runSpeed)
        {
            runSpeed = DistanceFormula(engine.Rating, tonnage);
            walkSpeed = runSpeed * 3 / 5;
            Control.mod.Logger.LogDebug("CalcSpeeds" +
                                        " rating=" + engine.Rating +
                                        " tonnage=" + tonnage +
                                        " walkSpeed=" + walkSpeed +
                                        " runSpeed=" + runSpeed);
        }

        internal int CalcInstallTechCost(Engine engine)
        {
            return Mathf.CeilToInt(Control.settings.TechCostPerEngineTon * engine.Def.Tonnage);
        }

        private static float DistanceFormula(int rating, float tonnage)
        {
            return rating / tonnage * factorwalkMPtoSprintDistance;
        }
    }

    internal enum EngineType
    {
        Std, XL
    }

    internal class Engine
    {
        internal EngineType Type;
        internal int Rating;
        internal MechComponentDef Def;

        private static readonly Regex EngineNameRegex = new Regex(@"^emod_engine_(\w+)_(\d+)$", RegexOptions.Compiled);

        internal static Engine MainEngineFromDef(MechComponentDef componentDef)
        {
            if (!IsMainEngine(componentDef))
            {
                return null;
            }

            var match = EngineNameRegex.Match(componentDef.Description.Id);
            return new Engine
            {
                Type = (EngineType)Enum.Parse(typeof(EngineType), match.Groups[1].Value, true),
                Rating = int.Parse(match.Groups[2].Value),
                Def = componentDef
            };
        }

        internal static bool IsEnginePart(MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, "emod_");
        }

        internal static bool IsMainEngine(MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, "emod_engine_");
        }

        internal static bool IsGryo(MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.Upgrade, "Gear_Gyro_");
        }

        private static bool CheckComponentDef(MechComponentDef componentDef, ComponentType type, string prefix)
        {
            if (componentDef == null || componentDef.Description == null || componentDef.Description.Id == null)
            {
                return false;
            }

            if (componentDef.ComponentType != type)
            {
                return false;
            }

            return componentDef.Description.Id.StartsWith(prefix);
        }

        public override string ToString()
        {
            return Def.Description.Id + " Type=" + Type + " Rating=" + Rating;
        }
    }

    public static class Control
    {
        public static Mod mod;

        public static MechEngineModSettings settings = new MechEngineModSettings();
        internal static EngineCalculator calc = new EngineCalculator();

        public static void Start(string modDirectory, string json)
        {
            mod = new Mod(modDirectory);
            Logger.SetLoggerLevel(mod.Logger.Name, LogLevel.Debug);
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
        
        // invalidate mech loadouts that don't have an engine
        [HarmonyPatch(typeof(MechValidationRules), "ValidateMechPosessesWeapons")]
        public static class MechValidationRulesPatch
        {
            public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
            {
                try
                {
                    var engineRefs = mechDef.Inventory.Where(x => Engine.IsEnginePart(x.Def)).ToList();
                    var mainEngine = engineRefs
                        .Where(x => x.DamageLevel == ComponentDamageLevel.Functional)
                        .Select(x => Engine.MainEngineFromDef(x.Def)).FirstOrDefault();
                    if (mainEngine == null)
                    {
                        errorMessages[MechValidationType.InvalidInventorySlots].Add("MISSING ENGINE: This Mech must mount a functional Fusion Engine");
                    }
                    else if (mainEngine.Type == EngineType.XL && engineRefs.Count(x => x.DamageLevel == ComponentDamageLevel.Functional) != 3)
                    {
                        errorMessages[MechValidationType.InvalidInventorySlots].Add("INCOMPLETE ENGINE: An XL Engine requires left and right torso components");
                    }
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }
        
        // only allow one engine in a specific location
        [HarmonyPatch(typeof(MechLabLocationWidget), "ValidateAdd", new[] { typeof(MechComponentDef) })]
        public static class MechLabLocationWidgetEnginePatch
        {
            public static void Postfix(MechLabLocationWidget __instance, MechComponentDef newComponentDef, ref bool __result)
            {
                try
                {
                    if (!__result)
                    {
                        return;
                    }

                    if (!Engine.IsEnginePart(newComponentDef))
                    {
                        return;
                    }

                    var adapter = new MechLabLocationWidgetAdapter(__instance);
                    if (adapter.LocalInventory.Select(x => x.ComponentRef).All(x => x == null || !Engine.IsEnginePart(x.Def)))
                    {
                        return;
                    }

                    adapter.DropErrorMessage = string.Format("Cannot add {0}: An engine part is already installed", newComponentDef.Description.Name);
                    __result = false;
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }

        // allow gyro upgrades to be 1 slot and still only be added once
        [HarmonyPatch(typeof(MechLabLocationWidget), "ValidateAdd", new[] { typeof(MechComponentDef) })]
        public static class MechLabLocationWidgetGyroPatch
        {
            public static void Postfix(MechLabLocationWidget __instance, MechComponentDef newComponentDef, ref bool __result)
            {
                try
                {
                    if (!__result)
                    {
                        return;
                    }

                    if (!Engine.IsGryo(newComponentDef))
                    {
                        return;
                    }

                    var adapter = new MechLabLocationWidgetAdapter(__instance);
                    if (adapter.LocalInventory.Select(x => x.ComponentRef).All(x => x == null || !Engine.IsGryo(x.Def)))
                    {
                        return;
                    }

                    adapter.DropErrorMessage = string.Format("Cannot add {0}: An engine upgrade is already installed", newComponentDef.Description.Name);
                    __result = false;
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }

        internal class MechLabLocationWidgetAdapter : Adapter<MechLabLocationWidget>
        {
            internal MechLabLocationWidgetAdapter(MechLabLocationWidget instance) : base(instance)
            {
            }

            internal List<MechLabItemSlotElement> LocalInventory
            {
                get { return traverse.Field("localInventory").GetValue() as List<MechLabItemSlotElement>; }
            }

            internal string DropErrorMessage
            {
                set { traverse.Field("dropErrorMessage").SetValue(value); }
            }
        }

        // make the mech movement summary stat be calculated using the engine
        [HarmonyPatch(typeof(MechStatisticsRules), "CalculateMovementStat")]
        public static class MechStatisticsRulesStatPatch
        {
            [HarmonyPriority(500)]
            public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
            {
                try
                {
                    var engine = mechDef.Inventory.Select(x => Engine.MainEngineFromDef(x.Def)).FirstOrDefault(x => x != null);
                    if (engine == null)
                    {
                        return true;
                    }

                    var maxTonnage = mechDef.Chassis.Tonnage;
                    //actualy tonnage is never used for anything in combat, hence it will warn you if you take less than the max
                    //since we don't want to fiddle around too much, ignore current tonnage
                    //var currentTonnage = 0f;
                    //MechStatisticsRules.CalculateTonnage(mechDef, ref currentTonnage, ref maxTonnage);
                    float walkSpeed, runSpeed;
                    calc.CalcSpeeds(engine, maxTonnage, out walkSpeed, out runSpeed);
                    var maxSprintDistance = runSpeed;

                    currentValue = Mathf.Floor(
                        (maxSprintDistance - UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinSprintFactor)
                        / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.MaxSprintFactor - UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinSprintFactor)
                        * 10f
                    );
                    currentValue = Mathf.Max(currentValue, 1f);
                    maxValue = 10f;
                    return false;
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                    return true;
                }
            }
        }

        // hide incompatible engines
        [HarmonyPatch(typeof(MechLabInventoryWidget), "RefreshJumpJetOptions")]
        public static class MechLabInventoryWidgetStatPatch
        {
            public static void Postfix(MechLabInventoryWidget __instance)
            {
                try
                {
                    var tonnage = Traverse.Create(__instance).Field("mechTonnage").GetValue<float>();
                    if (tonnage <= 0)
                    {
                        return;
                    }

                    foreach (var element in __instance.localInventory)
                    {
                        MechComponentDef componentDef;
                        if (element.controller != null)
                        {
                            componentDef = element.controller.componentDef;
                        }
                        else if (element.ComponentRef != null)
                        {
                            componentDef = element.ComponentRef.Def;
                        }
                        else
                        {
                            continue;
                        }

                        var engine = Engine.MainEngineFromDef(componentDef);
                        if (engine == null)
                        {
                            continue;
                        }
                        
                        if (calc.CalcAvailability(engine, tonnage))
                        {
                            continue;
                        }

                        element.gameObject.SetActive(false);
                    }
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }

        // change the movement stats when loading into a combat game the first time
        [HarmonyPatch(typeof(Mech), "InitEffectStats")]
        public static class MechPatch
        {
            public static void Postfix(Mech __instance)
            {
                try
                {
                    var engine = __instance.MechDef.Inventory
                        .Select(x => Engine.MainEngineFromDef(x.Def))
                        .FirstOrDefault(x => x != null);

                    if (engine == null)
                    {
                        return;
                    }
                    
                    var tonnage = __instance.tonnage;

                    float walkSpeed, runSpeed;
                    calc.CalcSpeeds(engine, tonnage, out walkSpeed, out runSpeed);

                    __instance.StatCollection.GetStatistic("WalkSpeed").SetValue(walkSpeed);
                    __instance.StatCollection.GetStatistic("RunSpeed").SetValue(runSpeed);
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }

        // change engine installation costs
        [HarmonyPatch(typeof(SimGameState), "CreateComponentInstallWorkOrder")]
        public static class SimGameStatePatch
        {
            public static void Postfix(SimGameState __instance, MechComponentRef mechComponent, ref WorkOrderEntry_InstallComponent __result)
            {
                try
                {
                    if (mechComponent == null)
                    {
                        return;
                    }

                    var engine = Engine.MainEngineFromDef(mechComponent.Def);
                    if (engine == null)
                    {
                        return;
                    }

                    __result.SetCost(calc.CalcInstallTechCost(engine));
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }
    }
}
