using System;
using System.Linq;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateMovementStat")]
    public static class EngineMechStatisticsRulesStatPatch
    {
        // make the mech movement summary stat be calculated using the engine
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
                Control.calc.CalcSpeeds(engine, maxTonnage, out walkSpeed, out runSpeed, out TTwalkSpeed);

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
                Control.mod.Logger.LogError(e);
                return true;
            }
        }
    }
}