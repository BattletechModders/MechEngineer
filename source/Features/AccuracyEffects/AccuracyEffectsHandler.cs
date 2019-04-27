using BattleTech;
using MechEngineer.Features.AccuracyEffects.Patches;
using MechEngineer.Features.LocationalEffects;
using MechEngineer.Misc;

namespace MechEngineer.Features.AccuracyEffects
{
    internal static class AccuracyEffectsHandler
    {
        internal static void SetupPatches()
        {
            FeatureUtils.SetupFeature(
                nameof(Features.AccuracyEffects),
                Control.settings.FeatureAccuracyEffectsEnabled,
                typeof(Patches.Mech_InitEffectStats_Patch),
                typeof(ToHit_GetSelfArmMountedModifier_Patch)
            );
        }

        internal static void SetupAccuracyStatistics(StatCollection statCollection)
        {
            foreach (var location in MechDefBuilder.Locations)
            {
                AccuracyForLocation(statCollection, location);
            }
        }
        
        internal static float AccuracyForLocation(StatCollection statCollection, ChassisLocations location)
        {
            var key = LocationalEffectsHandler.LocationalStatisticName("Accuracy", location);
            return AccuracyForKey(statCollection, key);
        }
        
        private static float AccuracyForKey(StatCollection statCollection, string collectionKey) 
        {
            var statistic = statCollection.GetStatistic(collectionKey);
            if (statistic == null)
            {
                const float defaultValue = 0.0f;
                statistic = statCollection.AddStatistic(collectionKey, defaultValue);
            }
            return statistic.Value<float>();
        }
    }
}
