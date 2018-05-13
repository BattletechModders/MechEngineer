
using System;
using System.Collections.Generic;
using System.Linq;
using HBS.Logging;
using Harmony;
using System.Reflection;
using BattleTech;
using BattleTech.UI;
using DynModLib;
using UnityEngine;
using Logger = HBS.Logging.Logger;

namespace MechEngineMod
{
    public class MechEngineModSettings : ModSettings
    {
        public int engineInstallTechCost = 5;
    }

    internal class EngineCalculator
    {
        internal bool CalcAvailability(int rating, float tonnage)
        {
            return true;
        }

        internal void CalcSpeeds(int rating, float tonnage, out float walkSpeed, out float runSpeed)
        {
            if (rating > 200)
            {
                walkSpeed = 200;
                runSpeed = 300;
            }
            else
            {
                walkSpeed = 100;
                runSpeed = 200;
            }
        }

        internal int CalcInstallTechCost(int rating)
        {
            return Control.settings.engineInstallTechCost;
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
            Logger.SetLoggerLevel(mod.Logger.Name, LogLevel.Log);
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
        
        internal static int? EngineRating(this MechComponentDef componentDef)
        {
            if (componentDef == null || componentDef.Description == null || componentDef.Description.Id == null)
            {
                return null;
            }

            if (!componentDef.Description.Id.StartsWith("emod_engine_"))
            {
                return null;
            }

            var id = componentDef.Description.Id;
            var rating = int.Parse(id.Substring(id.LastIndexOf("_") + 1));
            return rating;
        }

        [HarmonyPatch(typeof(MechValidationRules), "ValidateMechPosessesWeapons")]
        public static class MechValidationRulesPatch
        {
            public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
            {
                try
                {
                    var engine = mechDef.Inventory.FirstOrDefault(x => EngineRating(x.Def) != null);
                    if (engine == null || engine.DamageLevel != ComponentDamageLevel.Functional)
                    {
                        errorMessages[MechValidationType.InvalidInventorySlots].Add("MISSING ENGINE: This Mech must mount a functional Fusion Engine");
                    }
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }
        
        [HarmonyPatch(typeof(MechLabLocationWidget), "ValidateAdd", new[] { typeof(MechComponentDef) })]
        public static class MechLabLocationWidgetPatch
        {
            public static void Postfix(MechLabLocationWidget __instance, MechComponentDef newComponentDef, ref bool __result)
            {
                try
                {
                    if (!__result)
                    {
                        return;
                    }

                    if (EngineRating(newComponentDef) == null)
                    {
                        return;
                    }

                    var adapter = new MechLabLocationWidgetAdapter(__instance);
                    var engine = adapter.LocalInventory.Select(x => x.ComponentRef).FirstOrDefault(x => x != null && EngineRating(x.Def) != null);
                    if (engine == null)
                    {
                        return;
                    }

                    adapter.DropErrorMessage = string.Format("Cannot add {0}: An engine is already installed", newComponentDef.Description.Name);
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

        [HarmonyPatch(typeof(MechStatisticsRules), "CalculateMovementStat")]
        public static class MechStatisticsRulesStatPatch
        {
            [HarmonyPriority(500)]
            public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
            {
                try
                {
                    var rating = mechDef.Inventory.Select(x => EngineRating(x.Def)).FirstOrDefault(r => r != null);

                    float maxSprintDistance;
                    if (rating == null)
                    {
                        maxSprintDistance = 5; // mechDef.Chassis.MovementCapDef.MaxSprintDistance;
                    }
                    else
                    {
                        var maxTonnage = mechDef.Chassis.Tonnage;
                        //actualy tonnage is never used for anything in combat, hence it will warn you if you take less than the max
                        //since we don't want to fiddle around too much, ignore current tonnage
                        //var currentTonnage = 0f;
                        //MechStatisticsRules.CalculateTonnage(mechDef, ref currentTonnage, ref maxTonnage);
                        float walkSpeed, runSpeed;
                        calc.CalcSpeeds(rating.Value, maxTonnage, out walkSpeed, out runSpeed);
                        maxSprintDistance = runSpeed;
                    }

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

        [HarmonyPatch(typeof(MechLabInventoryWidget), "RefreshJumpJetOptions")]
        public static class MechLabInventoryWidgetStatPatch
        {
            public static void Postfix(MechLabInventoryWidget __instance)
            {
                try
                {
                    var tonnage = Traverse.Create(__instance).Field("mechTonnage").GetValue<float>();
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

                        if (componentDef == null)
                        {
                            continue;
                        }

                        var rating = EngineRating(componentDef);
                        if (rating == null)
                        {
                            continue;
                        }
                        
                        if (calc.CalcAvailability(rating.Value, tonnage))
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

        [HarmonyPatch(typeof(Mech), "InitEffectStats")]
        public static class MechPatch
        {
            public static void Postfix(Mech __instance)
            {
                try
                {
                    var rating = __instance.MechDef.Inventory.Select(x => EngineRating(x.Def)).FirstOrDefault(r => r != null);

                    if (rating == null)
                    {
                        return;
                    }
                    
                    var tonnage = __instance.tonnage;

                    float walkSpeed, runSpeed;
                    calc.CalcSpeeds(rating.Value, tonnage, out walkSpeed, out runSpeed);

                    __instance.StatCollection.GetStatistic("WalkSpeed").SetValue(walkSpeed);
                    __instance.StatCollection.GetStatistic("RunSpeed").SetValue(runSpeed);
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(SimGameStatePatch), "CreateComponentInstallWorkOrder")]
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
                    var rating = EngineRating(mechComponent.Def);
                    if (rating == null)
                    {
                        return;
                    }

                    __result.SetCost(calc.CalcInstallTechCost(rating.Value));
                }
                catch (Exception e)
                {
                    mod.Logger.LogError(e);
                }
            }
        }
    }
}
