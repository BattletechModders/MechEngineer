using BattleTech;
using UnityEngine;

namespace MechEngineer.Features.MoveMultiplierStat;

internal class MoveMultiplierStatFeature : Feature<MoveMultiplierStatSettings>
{
    internal static readonly MoveMultiplierStatFeature Shared = new();

    internal override MoveMultiplierStatSettings Settings => Control.settings.MoveMultiplierStat;

    internal void InitEffectStats(Mech mech)
    {
        mech.StatCollection.MoveMultiplier().Create();
    }

    internal void MoveMultiplier(Mech mech, ref float multiplier)
    {
        var multiplierStat = mech.StatCollection.MoveMultiplier().Get();
        var rounded = Mathf.Max(mech.Combat.Constants.MoveConstants.MinMoveSpeed, multiplierStat);
        multiplier *= rounded;
    }
}