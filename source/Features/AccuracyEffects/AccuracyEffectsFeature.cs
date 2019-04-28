using System;
using BattleTech;
using MechEngineer.Features.AccuracyEffects.Patches;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.LocationalEffects;

namespace MechEngineer.Features.AccuracyEffects
{
    internal class AccuracyEffectsFeature : Feature
    {
        internal static AccuracyEffectsFeature Shared = new AccuracyEffectsFeature();

        internal override bool Enabled => LocationalEffectsFeature.Shared.Loaded && Control.settings.FeatureAccuracyEffectsEnabled;
        internal override string Topic => nameof(Features.AccuracyEffects);
        internal override Type[] Patches => new[]
        {
            typeof(ToHit_GetSelfArmMountedModifier_Patch)
        };

        internal static void SetupAccuracyStatistics(StatCollection statCollection)
        {
            foreach (var location in MechDefBuilder.Locations)
            {
                AccuracyForLocation(statCollection, location);
            }
        }
        
        internal static float AccuracyForLocation(StatCollection statCollection, ChassisLocations location)
        {
            var key = LocationalEffectsFeature.LocationalStatisticName("Accuracy", location);
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
