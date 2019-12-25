using BattleTech;
using MechEngineer.Features.Engines.Helper;
using UnityEngine;

namespace MechEngineer.Features.Engines.StaticHandler
{
    internal class Jumping
    {
        internal static void InitEffectStats(Mech mech)
        {
            mech.StatCollection.JumpCapacity().Create();
            mech.StatCollection.JumpHeat().Create();
        }

        internal static int CalcJumpHeat(Mech mech, float jumpDistance)
        {
            var jumpCapacity = mech.StatCollection.JumpCapacity().Get();
            var maxJumpDistance = EngineMovement.ConvertMPToGameDistance(jumpCapacity);

            jumpDistance = Mathf.Max(jumpDistance, EngineFeature.settings.MinimumJumpDistanceForHeat);
            var jumpRatio = jumpDistance / maxJumpDistance;
            
            var jumpMaxHeat = mech.StatCollection.JumpHeat().Get();
            var jumpHeat = jumpRatio * jumpMaxHeat;
            return Mathf.CeilToInt(jumpHeat);
        }

        internal static float CalcMaxJumpDistance(Mech mech)
        {
            if (!mech.IsOperational || mech.IsProne)
			{
                return 0f;
			}

            var jumpCapacity = mech.StatCollection.JumpCapacity().Get();
            if (jumpCapacity < 0.1)
            {
                return 0f;
            }
            var jumpjetDistance = EngineMovement.ConvertMPToGameDistance(jumpCapacity);

            var mechJumpDistanceMultiplier = mech.StatCollection.JumpDistanceMultiplier().Get();
            return jumpjetDistance * mechJumpDistanceMultiplier;
        }
    }
}