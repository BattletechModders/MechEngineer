using BattleTech;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.Engines.Helper
{
    internal class EngineMovement
    {
        internal EngineMovement(int rating, float tonnage)
        {
            WalkMaxMovementPoint = rating / tonnage;
        }

        internal EngineMovement(float walkMovementPoint)
        {
            WalkMaxMovementPoint = walkMovementPoint;
        }

        internal float WalkMaxMovementPoint { get; }

        internal float WalkMaxSpeed => ConvertMPToGameDistance(WalkMaxMovementPoint);
        internal float RunMaxMovementPoint => WalkMaxMovementPoint * EngineFeature.settings.RunMultiplier;
        internal float RunMaxSpeed => ConvertMPToGameDistance(RunMaxMovementPoint);
        internal int JumpJetMaxCount => Mathf.FloorToInt(WalkMaxMovementPoint);

        internal bool Mountable
        {
            get
            {
                if (EngineFeature.settings.AllowMountingAllRatings)
                {
                    return true;
                }

                var constants = UnityGameInstance.BattleTechGame.MechStatisticsConstants;

                if (RunMaxSpeed < constants.MinSprintFactor)
                {
                    return false;
                }

                {
                    var lowerTier = new EngineMovement(Mathf.FloorToInt(WalkMaxMovementPoint) - 1);
                    if (lowerTier.RunMaxSpeed >= constants.MaxSprintFactor)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        internal static float ConvertMPToGameDistance(float movementPoints) {
            var multiplier = EngineFeature.settings.MovementPointDistanceMultiplier;
            return RoundBy5(movementPoints * multiplier);
        }

        private static float RoundBy5(float value)
        {
            return PrecisionUtils.RoundDown(value, 5);
        }
    }
}