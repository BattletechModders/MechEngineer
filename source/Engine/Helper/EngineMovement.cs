using System;
using BattleTech;
using UnityEngine;

namespace MechEngineer
{
    internal class EngineMovement
    {
        internal EngineMovement(int rating, float tonnage)
        {
            MovementPoint = rating / tonnage;
            WalkSpeed = CalcWalkDistance();
            RunSpeed = CalcSprintDistance();
            JumpJetCount = CalcJumpJets();
        }

        internal float MovementPoint { get; }
        internal float WalkSpeed { get; }
        internal float RunSpeed { get; }
        internal int JumpJetCount { get; }

        internal bool Mountable
        {
            get
            {
                if (RunSpeed < UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinSprintFactor)
                {
                    return false;
                }

                if (RunSpeed > UnityGameInstance.BattleTechGame.MechStatisticsConstants.MaxSprintFactor)
                {
                    return false;
                }

                return true;
            }
        }

        private int CalcJumpJets()
        {
            return Control.settings.CBTMovement.JJRoundUp ? Mathf.CeilToInt(MovementPoint) : Mathf.FloorToInt(MovementPoint);
        }

        private float CalcWalkDistance()
        {
            // numbers the result of the best fit line for the game movement
            var WalkSpeedFixed = 26.05f;
            var WalkSpeedMult = 23.14f;

            if (Control.settings.CBTMovement.UseGameWalkValues)
            {
                return (WalkSpeedFixed + MovementPoint * WalkSpeedMult).RoundBy5();
            }

            return (MovementPoint * Control.settings.CBTMovement.TTWalkMultiplier).RoundBy5();
        }

        private float CalcSprintDistance()
        {
            // numbers the result of the best fit line for the game movement
            var RunSpeedFixed = 52.43f;
            var RunSpeedMult = 37.29f;

            if (Control.settings.CBTMovement.UseGameWalkValues)
            {
                return (RunSpeedFixed + MovementPoint * RunSpeedMult).RoundBy5();
            }

            return (MovementPoint * Control.settings.CBTMovement.TTSprintMultiplier).RoundBy5();
        }
    }
}