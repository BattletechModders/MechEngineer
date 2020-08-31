﻿using BattleTech;
using MechEngineer.Features.OverrideTonnage;

namespace MechEngineer.Features.Engines.Helper
{
    internal class EngineMovement
    {
        internal EngineMovement(int rating, float tonnage)
        {
            WalkMovementPoint = RoundWalkMovementPoints(rating / tonnage);
        }

        private EngineMovement(float walkMovementPoint)
        {
            WalkMovementPoint = RoundWalkMovementPoints(walkMovementPoint);
        }

        internal float WalkMovementPoint { get; }

        internal float WalkSpeed => EngineFeature.settings.AdditionalWalkSpeed + ConvertMPToGameDistance(WalkMovementPoint);
        internal float RunMovementPoint => RoundRunMovementPoints(WalkMovementPoint * EngineFeature.settings.RunMultiplier);
        internal float RunSpeed => EngineFeature.settings.AdditionalRunSpeed + ConvertMPToGameDistance(RunMovementPoint);
        internal int JumpJetCount => PrecisionUtils.RoundDownToInt(WalkMovementPoint);

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

                {
                    var lowerTier = new EngineMovement(PrecisionUtils.RoundDownToInt(WalkMovementPoint) - 1);
                    if (lowerTier.RunSpeed >= constants.MaxSprintFactor)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        internal static float ConvertMPToGameDistance(float movementPoints) {
            var multiplier = EngineFeature.settings.MovementPointDistanceMultiplier;
            return RoundBy1(movementPoints * multiplier);
        }

        internal static float ConvertJJMPToGameDistance(float movementPoints) {
            var multiplier = EngineFeature.settings.JumpJetMovementPointDistanceMultiplier;
            if (multiplier.HasValue)
            {
                return RoundBy1(movementPoints * multiplier.Value);
            }
            return ConvertMPToGameDistance(movementPoints);
        }

        private static float RoundWalkMovementPoints(float value)
        {
            if (EngineFeature.settings.CBTWalkAndRunMPRounding)
            {
                return PrecisionUtils.RoundDownToInt(value);
            }
            return value;
        }

        private static float RoundRunMovementPoints(float value)
        {
            if (EngineFeature.settings.CBTWalkAndRunMPRounding)
            {
                return PrecisionUtils.RoundUpToInt(value);
            }
            return value;
        }

        private static float RoundBy1(float value)
        {
            return PrecisionUtils.RoundDown(value, 1);
        }
    }
}