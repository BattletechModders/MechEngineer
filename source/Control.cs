
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
        public int FallbackHeatSinkCapacity = 30;
        public float SpeedMultiplierPerDamagedEnginePart = 0.7f;
		
		//start Crusher Bob Additions
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
		//end Crusher Bob Additions
		
    }
	


    internal class EngineCalculator
    {
        // r = t * w
        // s = w * 5/3
        // f = d * t / r
        // CDA-2A 350d 40t 320r // 44f
        // SHD-2H 240d 55t 275r // 48f
        // AS7-D 165d 100t 300r // 55f
		
		//Crusher Bob: this value depreciated, use const_TTWalkMultiplier and const_TTSprintMultiplier instead
		/*
        internal static float factorwalkMPtoSprintDistance = 50;
		*/
		//Crusher Bob: end removed original code
		
		//Crusher Bob: Removed original code
		//use const_MinTTWalk and const_MaxTTWalk instead; sidesteps problems with curved vs non curved walk/sprint values
		/*
        internal static float MaxSprintFactor
        {
            get
            {
                //"MaxSprintFactor" : 350.0
				//Crusher Bob comment: is set in MechStatisticsConstants.json
                return Control.settings.MaxSprintFactor ?? UnityGameInstance.BattleTechGame.MechStatisticsConstants.MaxSprintFactor;
            }
        }

        internal static float MinSprintFactor
        {
            get
            {
                //"MinSprintFactor" : 125.0
				//Crusher Bob comment: is set in MechStatisticsConstants.json
                return Control.settings.MinSprintFactor ?? UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinSprintFactor;
            }
        }
		*/
		//Crusher Bob: end removed original code
		
		//start Crusher Bob Additions
		internal static float func_Roundby5(float value)
		/*
			examples:
				func_Roundby5(2.49) = 0
				func_Roundby5(2.5) = 5
				func_Roundby5(7.49) = 5
				func_Roundby5(7.5) = 10
		*/
		{
			if (value % 5f < 2.5)
				return (value - (value % 5f));
			else
				return (value - (value % 5f) + 5f);
		}
		//end Crusher Bob Additions

        internal bool CalcAvailability(Engine engine, float tonnage)
        {
            Control.mod.Logger.LogDebug("CalcAvailability rating=" + engine.Rating + " tonnage=" + tonnage);
            //// MWO style max rating
            //if (engine.Rating > tonnage * 8.5)
            //{
            //    return false;
            //}

			//Crusher Bob: removed original code
			/*
            var sprintDistance = DistanceFormula(engine.Rating, tonnage);
			*/
			//Crusher Bob: end removed original code
			
			//start Crusher Bob Additions
			float TTWalkDistance = engine.Rating / tonnage;
			//end Crusher Bob Additions

            Control.mod.Logger.LogDebug("CalcAvailability" +
                                        " rating=" + engine.Rating +
                                        " tonnage=" + tonnage +
										//Crusher Bob: removed original code
										/*
                                        " sprintDistance=" + sprintDistance +
                                        " MaxSprintFactor=" + MaxSprintFactor +
                                        " MinSprintFactor=" + MinSprintFactor);
										*/
										//Crusher Bob: end removed original code
										//start Crusher Bob Additions
                                        " TTWalkDistance=" + TTWalkDistance +
										" Min Walk =" + Control.settings.const_MinTTWalk +
										" Max Walk =" + Control.settings.const_MaxTTWalk);
										//end Crusher Bob Additions

			//Crusher Bob: Removed original code
			//checks now based on const_MinTTWalk and const_MaxTTWalk instead
			/*
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
			*/
			//Crusher Bob: end removed original code
			
			//start Crusher Bob Additions

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
			//end Crusher Bob Additions

            return true;
        }
		//Crusher Bob: removed original code
		/*
        internal void CalcSpeeds(Engine engine, float tonnage, out float walkSpeed, out float runSpeed)
		*/
		//Crusher Bob: end removed original code
			
		//start Crusher Bob Additions
		internal void CalcSpeeds(Engine engine, float tonnage, out float walkSpeed, out float runSpeed, out float TTWalkSpeed)
		//end Crusher Bob Additions
        {

			//Crusher Bob: removed original code
			/*
			runSpeed = DistanceFormula(engine.Rating, tonnage);
            walkSpeed = runSpeed * 3 / 5;
			*/
			//Crusher Bob: end removed original code
			
			//start Crusher Bob Additions
			TTWalkSpeed = engine.Rating / tonnage;
			walkSpeed = Calc_WalkDistance(TTWalkSpeed);
			runSpeed = Calc_SprintDistance(TTWalkSpeed);
			//end Crusher Bob Additions

			
            Control.mod.Logger.LogDebug("CalcSpeeds" +
                                        " rating=" + engine.Rating +
                                        " tonnage=" + tonnage +
                                        " walkSpeed=" + walkSpeed +
                                        " runSpeed=" + runSpeed 
										//start Crusher Bob Additions
										+
										" TTWalkSpeed=" + TTWalkSpeed)
										//end Crusher Bob Additions
										;
        }

        internal int CalcInstallTechCost(Engine engine)
        {
            return Mathf.CeilToInt(Control.settings.TechCostPerEngineTon * engine.Def.Tonnage);
        }
		
        internal int CalcJumpJetCount(Engine engine, float tonnage)
        {
		
			//Crusher Bob: Removed original code
			//Now just base the allowed number of jump jets on the calculated TT walk speed
            /*
			var runSpeed = DistanceFormula(engine.Rating, tonnage);
            var percentOfMaxSpeed = (runSpeed - MinSprintFactor) / (MaxSprintFactor - MinSprintFactor);

            var maxSpeedByTTRules = 8;
			
			return Mathf.CeilToInt(percentOfMaxSpeed * maxSpeedByTTRules);
			*/
			//Crusher Bob: end removed original code
			
			//start Crusher Bob Additions
			float TTWalkSpeed = engine.Rating / tonnage;
			float AllowedJJs = Math.Min(TTWalkSpeed, Control.settings.const_MaxNumberOfJJ);
			
			if (Control.settings.JJRoundUp == true)
			//end Crusher Bob Additions
				return Mathf.CeilToInt(AllowedJJs);
			//start Crusher Bob Additions
			else
				return Mathf.FloorToInt(AllowedJJs);
			//end Crusher Bob Additions
        }
		
		//Crusher Bob: this function depreciated, use Calc_WalkDistance and Calc_SprintDistance instead
		/*
        private static float DistanceFormula(int rating, float tonnage)
        {
            return rating / tonnage * factorwalkMPtoSprintDistance;
        }
		*/
		//Crusher Bob: end removed original code

		//start Crusher Bob Additions
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
		//end Crusher Bob Additions
		
    }

    internal enum EngineType
    {
		//Crusher Bob: removed original code
		/*
		Std, XL
		*/
		//Crusher Bob: end removed original code

		//start Crusher Bob Additions
		//making provisions for engines with single or double heat sinks
		Std_shs, Std_dhs, XL_shs, XL_dhs
		//end Crusher Bob Additions
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

		//Crusher Bob: this function depreciated. Use chassis def files instead, to allow for endo steel, etc
		/*
        // set initial weight of mechs to 0.1 times the tonnage
        [HarmonyPatch(typeof(ChassisDef), "FromJSON")]
        public static class ChassisDefPatch
        {
            public static void Postfix(ChassisDef __instance)
            {
                try
                {
                    var value = __instance.Tonnage * 0.1;
                    var propInfo = typeof(ChassisDef).GetProperty("InitialTonnage");
                    var propValue = Convert.ChangeType(value, propInfo.PropertyType);
                    propInfo.SetValue(__instance, propValue, null);
                    //Traverse.Create(__instance).Property("InitialTonnage").SetValue(value);
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }
		*/
		//Crusher Bob: end removed original code

        // invalidate mech loadouts that don't have an engine
		// invalidate mech loadouts that have more jump jets than the engine supports
        [HarmonyPatch(typeof(MechValidationRules), "ValidateMechPosessesWeapons")]
        public static class MechValidationRulesPatch
        {
            public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
            {
                try
                {
                    var engineRefs = mechDef.Inventory.Where(x => Engine.IsEnginePart(x.Def)).ToList();
                    var mainEngine = engineRefs
                        .Where(x => x.DamageLevel == ComponentDamageLevel.Functional || x.DamageLevel == ComponentDamageLevel.NonFunctional)
                        .Select(x => Engine.MainEngineFromDef(x.Def)).FirstOrDefault();
                    if (mainEngine == null)
                    {
                        errorMessages[MechValidationType.InvalidInventorySlots].Add("MISSING ENGINE: This Mech must mount a functional Fusion Engine");
                    }
					//Crusher Bob: removed original code
					/*
                    else if (mainEngine.Type == EngineType.XL && engineRefs.Count(x => x.DamageLevel ==
					*/
					//Crusher Bob: end removed original code
					
					//start Crusher Bob additions
					else if ((mainEngine.Type == EngineType.XL_shs || mainEngine.Type == EngineType.XL_dhs )&& engineRefs.Count(x => x.DamageLevel ==
					//end Crusher Bob additions
					ComponentDamageLevel.Functional || x.DamageLevel == ComponentDamageLevel.NonFunctional) != 3)
                    {
                        errorMessages[MechValidationType.InvalidInventorySlots].Add("INCOMPLETE ENGINE: An XL Engine requires left and right torso components");
                    }
					
                    var currentCount = mechDef.Inventory.Count(c => c.ComponentDefType == ComponentType.JumpJet);
                    var maxCount = calc.CalcJumpJetCount(mainEngine, mechDef.Chassis.Tonnage); ;
                    if (currentCount > maxCount)
                    {
                        errorMessages[MechValidationType.InvalidJumpjets].Add(string.Format("JUMPJETS: This 'Mech mounts too many jumpjets ({0} out of {1})", currentCount, maxCount));
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
                    float walkSpeed, runSpeed, TTwalkSpeed;
                    calc.CalcSpeeds(engine, maxTonnage, out walkSpeed, out runSpeed, out TTwalkSpeed);
                    var maxSprintDistance = runSpeed;

					//Crusher Bob: removed original code
					/*
                    currentValue = Mathf.Floor(
                        (maxSprintDistance - UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinSprintFactor)
                        / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.MaxSprintFactor - UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinSprintFactor)
                        * 10f
                    );
					*/
					//Crusher Bob: end removed original code

					//start Crusher Bob additions
					//used much more familiar TT walk speed values
					//also, user doesn't have to change min/max sprint values depending on if they are using curved movement or not
					//divided by 9 instead of 10 to make scale more reactive at the bottom.
                    currentValue = Mathf.Floor(
                        (TTwalkSpeed - Control.settings.const_MinTTWalk)
                        / (Control.settings.const_MaxTTWalk - Control.settings.const_MinTTWalk)
                        * 9f +1
                    );
					//end Crusher Bob additions
					
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
