using BattleTech;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.Engines.Helper
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
                if (EngineFeature.settings.AllowMountingAllRatings)
                {
                    return true;
                }

                var constants = UnityGameInstance.BattleTechGame.MechStatisticsConstants;

                if (RunSpeed < constants.MinSprintFactor)
                {
                    return false;
                }

                if (RunSpeed > constants.MaxSprintFactor)
                {
                    return false;
                }

                return true;
            }
        }

        private int CalcJumpJets()
        {
            return EngineFeature.settings.JJRoundUp ? Mathf.CeilToInt(MovementPoint) : Mathf.FloorToInt(MovementPoint);
        }

        private float CalcWalkDistance()
        {
            // numbers the result of the best fit line for the game movement
            var WalkSpeedFixed = 26.05f;
            var WalkSpeedMult = 23.14f;

            if (EngineFeature.settings.UseGameWalkValues)
            {
                return RoundBy5(WalkSpeedFixed + MovementPoint * WalkSpeedMult);
            }

            return RoundBy5(MovementPoint * EngineFeature.settings.TTWalkMultiplier);
        }

        private float CalcSprintDistance()
        {
            // numbers the result of the best fit line for the game movement
            var RunSpeedFixed = 52.43f;
            var RunSpeedMult = 37.29f;

            if (EngineFeature.settings.UseGameWalkValues)
            {
                return RoundBy5(RunSpeedFixed + MovementPoint * RunSpeedMult);
            }

            return RoundBy5(MovementPoint * EngineFeature.settings.TTSprintMultiplier);
        }

        private static float RoundBy5(float value)
        {
            return PrecisionUtils.RoundDown(value, 5);
        }
    }
}