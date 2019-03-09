using System;
using BattleTech;

namespace MechEngineer
{
    public static class AccuracyEffects
    {
        public static void SetupAccuracyStatistics(StatCollection statCollection)
        {
            foreach (var location in MechDefBuilder.Locations)
            {
                AccuracyForLocation(statCollection, location);
            }
        }
        
        public static float AccuracyForLocation(StatCollection statCollection, ChassisLocations location)
        {
            var key = LocationalEffects.LocationalStatisticName("Accuracy", location);
            return AccuracyForKey(statCollection, key);
        }
        
        private static float AccuracyForKey(StatCollection statCollection, string collectionKey) 
        {
            var statistic = statCollection.GetStatistic(collectionKey);
            if (statistic == null)
            {
                const float defaultValue = 1.0f;
                statistic = statCollection.AddStatistic(collectionKey, defaultValue);
            }
            return statistic.Value<float>();
        }
    }
}
