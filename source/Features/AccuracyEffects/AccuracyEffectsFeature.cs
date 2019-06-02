using BattleTech;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.LocationalEffects;

namespace MechEngineer.Features.AccuracyEffects
{
    internal class AccuracyEffectsFeature : Feature<BaseSettings>
    {
        internal static AccuracyEffectsFeature Shared = new AccuracyEffectsFeature();

        internal override BaseSettings Settings => Control.settings.AccuracyEffects;

        internal override bool Enabled => base.Enabled && LocationalEffectsFeature.Shared.Loaded;

        internal static void SetupAccuracyStatistics(StatCollection statCollection)
        {
            foreach (var location in MechDefBuilder.Locations)
            {
                AccuracyForLocation(statCollection, location);
            }
        }
        
        internal static float AccuracyForLocation(StatCollection statCollection, ChassisLocations location)
        {
            var naming = new MechLocationNaming(location);
            var key = naming.LocationalStatisticName("Accuracy");
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
