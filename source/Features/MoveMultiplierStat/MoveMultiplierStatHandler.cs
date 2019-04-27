using BattleTech;
using MechEngineer.Features.MoveMultiplierStat.Patches;
using MechEngineer.Misc;
using UnityEngine;

namespace MechEngineer.Features.MoveMultiplierStat
{
    internal static class MoveMultiplierStatHandler
    {
        internal static void SetupPatches()
        {
            FeatureUtils.SetupFeature(
                nameof(Features.MoveMultiplierStat),
                Control.settings.FeatureMoveMultiplierEnabled,
                typeof(Mech_InitEffectStats_Patch),
                typeof(Mech_MoveMultiplier_Patch)
            );
        }

        internal static void InitEffectStats(Mech mech)
        {
            MoveMultiplierStat(mech.StatCollection);
        }

        internal static void MoveMultiplier(Mech mech, ref float multiplier)
        {
            var multiplierStat = MoveMultiplierStat(mech.StatCollection);
            var rounded = Mathf.Max(mech.Combat.Constants.MoveConstants.MinMoveSpeed, multiplierStat);
            multiplier *= rounded;
        }

        private static float MoveMultiplierStat(StatCollection statCollection) 
        {
            const string key = "MoveMultiplier";
            var statistic = statCollection.GetStatistic(key);
            if (statistic == null)
            {
                const float defaultValue = 1.0f;
                statistic = statCollection.AddStatistic(key, defaultValue);
            }
            return statistic.Value<float>();
        }
    }
}
