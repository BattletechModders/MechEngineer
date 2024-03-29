namespace MechEngineer.Features.ArmorMaximizer;

internal class ArmorMaximizerFeature : Feature<ArmorMaximizerSettings>
{
    internal static readonly ArmorMaximizerFeature Shared = new();

    internal override ArmorMaximizerSettings Settings => Control.Settings.ArmorMaximizer;

    protected override void SetupFeatureLoaded()
    {
        foreach (var location in Settings.ArmorLocationsLockedByDefault)
        {
            ArmorLocationLocker.ToggleLock(location);
        }
    }
}