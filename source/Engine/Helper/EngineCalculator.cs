using System;
using BattleTech;
using UnityEngine;

namespace MechEngineer
{
    internal class EngineCalculator
    {
        internal float CalcMovementPoints(EngineCoreDef engineCoreDef, float tonnage)
        {
            return engineCoreDef.Rating / tonnage;
        }

        internal bool CalcAvailability(EngineCoreDef engineCoreDef, float tonnage)
        {
            var mp = CalcMovementPoints(engineCoreDef, tonnage);
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

        internal void CalcSpeeds(EngineCoreDef engineCoreDef, float tonnage, out float walkSpeed, out float runSpeed)
        {
            var mp = CalcMovementPoints(engineCoreDef, tonnage);
            walkSpeed = CalcWalkDistance(mp);
            runSpeed = CalcSprintDistance(mp);
        }

        internal void CalcInstallCosts(EngineCoreDef engineCoreDef, ref int cbillCost, ref int techCost)
        {
            cbillCost += Mathf.CeilToInt(engineCoreDef.Tonnage * 10000);
            techCost += Mathf.CeilToInt(engineCoreDef.Tonnage);
        }

        internal void CalcHeatSinks(EngineCoreDef engineCoreDef, out int minHeatSinks, out int maxHeatSinks)
        {
            maxHeatSinks = engineCoreDef.Rating / 25;
            minHeatSinks = Math.Min(maxHeatSinks, 10);
        }

        internal float CalcGyroWeight(EngineCoreDef engineCoreDef)
        {
            return (engineCoreDef.Rating / 100.0f).RoundStandard();
        }

        internal int CalcJumpJetCount(EngineCoreDef engineCoreDef, float tonnage)
        {
            var mp = CalcMovementPoints(engineCoreDef, tonnage);
            return Control.settings.JJRoundUp ? Mathf.CeilToInt(mp) : Mathf.FloorToInt(mp);
        }

        private static float CalcWalkDistance(float TTWalkSpeed)
        {
            // numbers the result of the best fit line for the game movement
            var WalkSpeedFixed = 26.05f;
            var WalkSpeedMult = 23.14f;

            if (Control.settings.UseGameWalkValues)
            {
                return (WalkSpeedFixed + TTWalkSpeed * WalkSpeedMult).RoundBy5();
            }

            return (TTWalkSpeed * Control.settings.const_TTWalkMultiplier).RoundBy5();
        }

        private static float CalcSprintDistance(float TTWalkSpeed)
        {
            // numbers the result of the best fit line for the game movement
            var RunSpeedFixed = 52.43f;
            var RunSpeedMult = 37.29f;

            if (Control.settings.UseGameWalkValues)
            {
                return (RunSpeedFixed + TTWalkSpeed * RunSpeedMult).RoundBy5();
            }

            return (TTWalkSpeed * Control.settings.const_TTSprintMultiplier).RoundBy5();
        }
    }
}