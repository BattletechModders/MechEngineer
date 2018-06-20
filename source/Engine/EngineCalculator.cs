using System;
using BattleTech;
using UnityEngine;

namespace MechEngineMod
{
    internal class EngineCalculator
    {
        internal float CalcMovementPoints(EngineDef engineDef, float tonnage)
        {
            return engineDef.Rating / tonnage;
        }

        internal bool CalcAvailability(EngineDef engineDef, float tonnage)
        {
            var mp = CalcMovementPoints(engineDef, tonnage);
            var sprint = CalcSprintDistance(mp);

            if (sprint < UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinSprintFactor)
            {
                return false;
            }

            if (sprint > UnityGameInstance.BattleTechGame.MechStatisticsConstants.MaxSprintFactor)
            {
                return false;
            }

            return true;
        }

        internal void CalcSpeeds(EngineDef engineDef, float tonnage, out float walkSpeed, out float runSpeed)
        {
            var mp = CalcMovementPoints(engineDef, tonnage);
            walkSpeed = CalcWalkDistance(mp);
            runSpeed = CalcSprintDistance(mp);
        }

        internal int CalcInstallTechCost(EngineDef engineDef)
        {
            return Mathf.CeilToInt(Control.settings.TechCostPerEngineTon * engineDef.Def.Tonnage);
        }

        internal void CalcHeatSinks(EngineDef engineDef, out int minHeatSinks, out int maxHeatSinks)
        {
            maxHeatSinks = engineDef.Rating / 25;
            minHeatSinks = Math.Min(maxHeatSinks, 10);
        }

        internal float CalcGyroWeight(EngineDef engineDef)
        {
            // for now only used for engine details text, not for any actual tonnage calculations
            return (int)(engineDef.Rating / 100);
        }

        internal int CalcJumpJetCount(EngineDef engineDef, float tonnage)
        {
            var mp = CalcMovementPoints(engineDef, tonnage);
            return Control.settings.JJRoundUp ? Mathf.CeilToInt(mp) : Mathf.FloorToInt(mp);
        }

        private static float CalcWalkDistance(float TTWalkSpeed)
            // numbers the result of the best fit line for the game movement
        {
            var WalkSpeedFixed = 26.05f;
            var WalkSpeedMult = 23.14f;
		
            if (Control.settings.UseGameWalkValues)
                return (WalkSpeedFixed + TTWalkSpeed * WalkSpeedMult).RoundBy5();
            else
                return (TTWalkSpeed * Control.settings.const_TTWalkMultiplier).RoundBy5();
			
        }
		
        private static float CalcSprintDistance(float TTWalkSpeed)
            // numbers the result of the best fit line for the game movement
        {
            var RunSpeedFixed = 52.43f;
            var RunSpeedMult = 37.29f;
			
            if (Control.settings.UseGameWalkValues)
                return (RunSpeedFixed + TTWalkSpeed * RunSpeedMult).RoundBy5();
            else
                return (TTWalkSpeed * Control.settings.const_TTSprintMultiplier).RoundBy5();
        }
		
    }
}