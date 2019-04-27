using BattleTech;
using Harmony;
using MechEngineer.Features.MoveMultiplierStat.Patches;
using MechEngineer.Misc;
using UnityEngine;

namespace MechEngineer.Features.MoveMultiplierStat
{
    internal class MoveMultiplierStatHandler
    {
        internal static void Setup(HarmonyInstance harmony, MechEngineerSettings settings)
        {
            var success = FeatureUtils.SetupFeature(
                harmony,
                nameof(Features.MoveMultiplierStat),
                settings.FeatureMoveMultiplierEnabled,
                typeof(Mech_InitEffectStats_Patch),
                typeof(Mech_MoveMultiplier_Patch)
            );

            if (success)
            {
                Shared = new MoveMultiplierStatHandler();
            }
        }

        internal static MoveMultiplierStatHandler Shared;

        internal void InitEffectStats(Mech mech)
        {
            MoveMultiplierStat(mech.StatCollection);
        }

        internal void MoveMultiplier(Mech mech, ref float multiplier)
        {
            var multiplierStat = MoveMultiplierStat(mech.StatCollection);
            var rounded = Mathf.Max(mech.Combat.Constants.MoveConstants.MinMoveSpeed, multiplierStat);
            multiplier = multiplier * rounded;
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
