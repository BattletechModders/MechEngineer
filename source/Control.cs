
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
        public int FallbackHeatSinkCapacity = 30;
        public float SpeedMultiplierPerDamagedEnginePart = 0.7f;
        public bool InitialTonnageOverride = false;
        public float InitialToTotalTonnageFactor = 0.1f;
        public string[] InitialTonnageOverrideSkipChassis = {};
        public int EndoSteelRequiredCriticals = 14;

        /* 
		set to false to use TT walk values
		using the default game values, slow mechs move a bit faster, and fast mechs move a bit slower
		Examples if set to true:
			Walk 2  70 / 125
			Walk 3  95 / 165
			Walk 4 120 / 200
			Walk 5 140 / 240
			Walk 6 165 / 275
			Walk 7 190 / 315
			Walk 8 210 / 350
		*/
        public bool UseGameWalkValues = true;
		
		// set to false to only allow engines that produce integer walk values
		public bool AllowNonIntWalkValues = true;
		
		// this setting controls if the allowed number of jump jets is rounded up or down
		// example: if false, TT walk speed of 2.1 allows 2 jump jets, if true, it allows 3 jump jets
		public bool JJRoundUp = false;
		
		// this controls the maximum (global) allowed jump jets.
		// currently set to 8 in case the game can't handle anymore
		public int const_MaxNumberOfJJ = 8;
		
		/*
		these numbers determine how fast and slow mechs are allowed to go
		They are given in tabletop walk numbers.
		Examples: 
			urbanmech walk = 2
			griffin walk = 5
			spider walk = 8
		*/
		public int const_MinTTWalk = 2;
		public int const_MaxTTWalk = 8;
		
		/*
		not sure why you would want to change these, but they are set here
		they are the multiples that translate TT movement values to game movement values
		Example:
			A griffin that walks 5 would walk 5 * 30 = 150 and sprint 5 * 50 = 250
		NOTE: if you have the UseGameWalkValues set, the exact values are then changed based on a linear equasion
		*/
		public float const_TTWalkMultiplier = 30f;
		public float const_TTSprintMultiplier = 50f;
		
    }
	


    internal class EngineCalculator
    {
        // r = t * w
        // s = w * 5/3
        // f = d * t / r
        // CDA-2A 350d 40t 320r // 44f
        // SHD-2H 240d 55t 275r // 48f
        // AS7-D 165d 100t 300r // 55f		
		internal static float func_Roundby5(float value)
		{
			if (value % 5f < 2.5)
				return (value - (value % 5f));
			else
				return (value - (value % 5f) + 5f);
		}

        internal bool CalcAvailability(Engine engine, float tonnage)
        {
            Control.mod.Logger.LogDebug("CalcAvailability rating=" + engine.Rating + " tonnage=" + tonnage);
			
			float TTWalkDistance = engine.Rating / tonnage;

            Control.mod.Logger.LogDebug("CalcAvailability" +
                                        " rating=" + engine.Rating +
                                        " tonnage=" + tonnage +
                                        " TTWalkDistance=" + TTWalkDistance +
										" Min Walk =" + Control.settings.const_MinTTWalk +
										" Max Walk =" + Control.settings.const_MaxTTWalk);

            // check if over max walk distance
            if (TTWalkDistance > Control.settings.const_MaxTTWalk)
            {
                return false;
            }

            // check if below min walk distance
            if (TTWalkDistance < Control.settings.const_MinTTWalk)
            {
                return false;
            }
			
			//check if non integer TT walk values are allowed
			if (Control.settings.AllowNonIntWalkValues == false)
			{
				//if not, check that walk value is an integer
				if ((TTWalkDistance % 1f) != 0)
					return false;
			}

            return true;
        }


		internal void CalcSpeeds(Engine engine, float tonnage, out float walkSpeed, out float runSpeed, out float TTWalkSpeed)
        {

			TTWalkSpeed = engine.Rating / tonnage;
			walkSpeed = Calc_WalkDistance(TTWalkSpeed);
			runSpeed = Calc_SprintDistance(TTWalkSpeed);

			
            Control.mod.Logger.LogDebug("CalcSpeeds" +
                                        " rating=" + engine.Rating +
                                        " tonnage=" + tonnage +
                                        " walkSpeed=" + walkSpeed +
                                        " runSpeed=" + runSpeed 
										+
										" TTWalkSpeed=" + TTWalkSpeed)
										;
        }

        internal int CalcInstallTechCost(Engine engine)
        {
            return Mathf.CeilToInt(Control.settings.TechCostPerEngineTon * engine.Def.Tonnage);
        }
        
        internal int CalcJumpJetCount(Engine engine, float tonnage)
        {

			float TTWalkSpeed = engine.Rating / tonnage;
			float AllowedJJs = Math.Min(TTWalkSpeed, Control.settings.const_MaxNumberOfJJ);
			
			if (Control.settings.JJRoundUp == true)
				return Mathf.CeilToInt(AllowedJJs);
			else
				return Mathf.FloorToInt(AllowedJJs);
        }

        private static float Calc_WalkDistance(float TTWalkSpeed)
		// numbers the result of the best fit line for the game movement
        {
			float WalkSpeedFixed = 26.05f;
			float WalkSpeedMult = 23.14f;
		
			if (Control.settings.UseGameWalkValues == true)
				return func_Roundby5(WalkSpeedFixed + TTWalkSpeed * WalkSpeedMult);
			else
				return func_Roundby5(TTWalkSpeed * Control.settings.const_TTWalkMultiplier);
			
		}
		
        private static float Calc_SprintDistance(float TTWalkSpeed)
		// numbers the result of the best fit line for the game movement
        {
			float RunSpeedFixed = 52.43f;
			float RunSpeedMult = 37.29f;
			
			if (Control.settings.UseGameWalkValues == true)
				return func_Roundby5(RunSpeedFixed + TTWalkSpeed * RunSpeedMult);
			else
				return func_Roundby5(TTWalkSpeed * Control.settings.const_TTSprintMultiplier);
		}
		
    }

    internal enum EngineType
    {
		//making provisions for engines with single or double heat sinks
		Std_shs, Std_dhs, XL_shs, XL_dhs
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

        // main engine + engine slots
        internal static bool IsEnginePart(MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, "emod_engine");
        }

        // only main engine
        internal static bool IsMainEngine(MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, "emod_engine_");
        }

        // we want to know about center torso upgrade (gyros), since we reduce their size, several could be added, and this makes sure only one can be added
        internal static bool IsCenterTorsoUpgrade(MechComponentDef componentDef)
        {
            return componentDef.AllowedLocations == ChassisLocations.CenterTorso && componentDef.ComponentType == ComponentType.Upgrade;
        }

        internal static bool IsEndoSteel(MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, "emod_structureslots_endosteel");
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

        // set initial weight of mechs to 0.1 times the tonnage
        [HarmonyPatch(typeof(ChassisDef), "FromJSON")]
        public static class ChassisDefPatch
        {
            public static void Postfix(ChassisDef __instance)
            {
                try
                {
                    if (!settings.InitialTonnageOverride)
                    {
                        return;
                    }

                    if (settings.InitialTonnageOverrideSkipChassis.Contains(__instance.Description.Id))
                    {
                        return;
                    }

                    var value = __instance.Tonnage * settings.InitialToTotalTonnageFactor;
                    var propInfo = typeof(ChassisDef).GetProperty("InitialTonnage");
                    var propValue = Convert.ChangeType(value, propInfo.PropertyType);
                    propInfo.SetValue(__instance, propValue, null);
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }

        // reduce upgrade components for the center torso that are 3 or larger 
        [HarmonyPatch(typeof(UpgradeDef), "FromJSON")]
        public static class UpgradeDefPatch
        {
            public static void Postfix(UpgradeDef __instance)
            {
                try
                {
                    if (!Engine.IsCenterTorsoUpgrade(__instance))
                    {
                        return;
                    }

                    if (__instance.InventorySize < 3)
                    {
                        return;
                    }

                    var value = __instance.InventorySize - 2;
                    var propInfo = typeof(ChassisDef).GetProperty("InventorySize");
                    var propValue = Convert.ChangeType(value, propInfo.PropertyType);
                    propInfo.SetValue(__instance, propValue, null);
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }

        // invalidate mech loadouts that don't have an engine
        // invalidate mech loadouts that have more jump jets than the engine supports
        // invalidate mech loadouts that have more than one endo-steel critical slot but not exactly 14
        [HarmonyPatch(typeof(MechValidationRules), "ValidateMechPosessesWeapons")]
        public static class MechValidationRulesPatch
        {
            public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
            {
                try
                {
                    // engine
                    var engineRefs = mechDef.Inventory.Where(x => Engine.IsEnginePart(x.Def)).ToList();
                    var mainEngine = engineRefs
                        .Where(x => x.DamageLevel == ComponentDamageLevel.Functional || x.DamageLevel == ComponentDamageLevel.NonFunctional)
                        .Select(x => Engine.MainEngineFromDef(x.Def)).FirstOrDefault();
                    if (mainEngine == null)
                    {
                        errorMessages[MechValidationType.InvalidInventorySlots].Add("MISSING ENGINE: This Mech must mount a functional Fusion Engine");
                    }
					else if ((mainEngine.Type == EngineType.XL_shs || mainEngine.Type == EngineType.XL_dhs ) && engineRefs.Count(x => x.DamageLevel == ComponentDamageLevel.Functional || x.DamageLevel == ComponentDamageLevel.NonFunctional) != 3)
                    {
                        errorMessages[MechValidationType.InvalidInventorySlots].Add("INCOMPLETE ENGINE: An XL Engine requires left and right torso components");
                    }

                    // jump jets
                    {
                        var currentCount = mechDef.Inventory.Count(c => c.ComponentDefType == ComponentType.JumpJet);
                        var maxCount = calc.CalcJumpJetCount(mainEngine, mechDef.Chassis.Tonnage); ;
                        if (currentCount > maxCount)
                        {
                            errorMessages[MechValidationType.InvalidJumpjets].Add(string.Format("JUMPJETS: This Mech mounts too many jumpjets ({0} out of {1})", currentCount, maxCount));
                        }
                    }

                    // endo-steel
                    {
                        var currentCount = mechDef.Inventory.Count(x => Engine.IsEndoSteel(x.Def));
                        var exactCount = settings.EndoSteelRequiredCriticals;
                        if (currentCount > 0 && currentCount != exactCount)
                        {
                            errorMessages[MechValidationType.InvalidInventorySlots].Add(string.Format("INCOMPLETE ENDO-STEEL: Critical slots count does not match ({0} instead of {1})", currentCount, exactCount));
                        }
                    }
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }

        // only allow one engine part per specific location
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

                    if (!Engine.IsCenterTorsoUpgrade(newComponentDef))
                    {
                        return;
                    }

                    var adapter = new MechLabLocationWidgetAdapter(__instance);
                    if (adapter.LocalInventory.Select(x => x.ComponentRef).All(x => x == null || !Engine.IsCenterTorsoUpgrade(x.Def)))
                    {
                        return;
                    }

                    adapter.DropErrorMessage = string.Format("Cannot add {0}: A center torso upgrade is already installed", newComponentDef.Description.Name);
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

        // include endo-steel calculations
        [HarmonyPatch(typeof(MechStatisticsRules), "CalculateTonnage")]
        public static class MechStatisticsRulesCalculateTonnagePatch
        {
            public static void Postfix(MechDef mechDef, ref float currentValue, ref float maxValue)
            {
                try
                {
                    if (mechDef.Inventory.Any(x => Engine.IsEndoSteel(x.Def)))
                    {
                        currentValue -= mechDef.Chassis.InitialTonnage / 2;
                    }
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
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
                    float walkSpeed, runSpeed, TTwalkSpeed;
                    calc.CalcSpeeds(engine, maxTonnage, out walkSpeed, out runSpeed, out TTwalkSpeed);

					//used much more familiar TT walk speed values
					//also, user doesn't have to change min/max sprint values depending on if they are using curved movement or not
					//divided by 9 instead of 10 to make scale more reactive at the bottom.
                    currentValue = Mathf.Floor(
                        (TTwalkSpeed - Control.settings.const_MinTTWalk)
                        / (Control.settings.const_MaxTTWalk - Control.settings.const_MinTTWalk)
                        * 9f +1
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
                        __instance.StatCollection.GetStatistic("HeatSinkCapacity").SetValue(settings.FallbackHeatSinkCapacity);
                        return;
                    }
                    
                    var tonnage = __instance.tonnage;

                    float walkSpeed, runSpeed, TTWalkSpeed;
                    calc.CalcSpeeds(engine, tonnage, out walkSpeed, out runSpeed, out TTWalkSpeed);

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

        // crit engine reduces speed
        // destroyed engine destroys CT
        [HarmonyPatch(typeof(MechComponent), "DamageComponent")]
        public static class MechComponentPatch
        {
            public static void Postfix(MechComponent __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
            {
                try
                {
                    if (__instance.componentDef == null || !(__instance.parent is Mech))
                    {
                        return;
                    }

                    if (!Engine.IsEnginePart(__instance.componentDef))
                    {
                        return;
                    }

                    var mech = (Mech)__instance.parent;
                    if (damageLevel == ComponentDamageLevel.Penalized)
                    {
                        if (!mech.IsLocationDestroyed(ChassisLocations.CenterTorso))
                        {
                            mod.Logger.LogDebug("Penalized=" + __instance.Location);

                            var walkSpeed = mech.StatCollection.GetStatistic("WalkSpeed");
                            var runSpeed = mech.StatCollection.GetStatistic("RunSpeed");
                            mech.StatCollection.Float_Multiply(walkSpeed, settings.SpeedMultiplierPerDamagedEnginePart);
                            mech.StatCollection.Float_Multiply(runSpeed, settings.SpeedMultiplierPerDamagedEnginePart);
                        }
                    }
                    else if (damageLevel == ComponentDamageLevel.Destroyed)
                    {
                        mod.Logger.LogDebug("Destroyed=" + __instance.Location);

                        if (!mech.IsLocationDestroyed(ChassisLocations.CenterTorso))
                        {
                            var onUnitSphere = UnityEngine.Random.onUnitSphere;
                            mech.NukeStructureLocation(hitInfo, __instance.Location, ChassisLocations.CenterTorso, onUnitSphere, false);
                        }
                    }
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }
    }
}
