using BattleTech;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.Engines.StaticHandler;

internal static class Jumping
{
    internal static void InitEffectStats(Mech mech)
    {
        mech.StatCollection.JumpCapacity().Create();
        mech.StatCollection.JumpHeat().Create();
    }

    internal static int CalcJumpHeat(Mech mech, float jumpDistance)
    {
        var jumpCapacity = mech.StatCollection.JumpCapacity().Get();
        var maxJumpDistance = EngineMovement.ConvertJJMPToGameDistance(jumpCapacity);
        var jumpRatio = jumpDistance / maxJumpDistance;

        return GetJumpHeat(mech.StatCollection, jumpRatio);
    }

    internal static int GetJumpHeat(this StatCollection statCollection, float ratio)
    {
        var jumpMaxHeat = statCollection.JumpHeat().Get();
        var jumpHeat = ratio * jumpMaxHeat;
        jumpHeat = Mathf.Max(jumpHeat, EngineFeature.settings.MinimJumpHeat);
        return PrecisionUtils.RoundUpToInt(jumpHeat);
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
        var jumpjetDistance = EngineMovement.ConvertJJMPToGameDistance(jumpCapacity);

        var mechJumpDistanceMultiplier = mech.StatCollection.JumpDistanceMultiplier().Get();
        return jumpjetDistance * mechJumpDistanceMultiplier;
    }
}