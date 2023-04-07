using BattleTech;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.PlaceholderEffects;

namespace MechEngineer.Features.AccuracyEffects;

internal class AccuracyEffectsFeature : Feature<AccuracyEffectsSettings>
{
    internal static readonly AccuracyEffectsFeature Shared = new();

    internal override AccuracyEffectsSettings Settings => Control.Settings.AccuracyEffects;

    internal override bool Enabled => base.Enabled && PlaceholderEffectsFeature.Shared.Loaded;

    internal static void SetupAccuracyStatistics(StatCollection statCollection)
    {
        foreach (var location in LocationUtils.Locations)
        {
            AccuracyForLocation(statCollection, location);
        }
    }

    internal static float AccuracyForLocation(StatCollection statCollection, ChassisLocations location)
    {
        var naming = new MechPlaceholderInterpolation(location);
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