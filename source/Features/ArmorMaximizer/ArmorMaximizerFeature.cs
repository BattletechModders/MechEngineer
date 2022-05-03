using BattleTech;

namespace MechEngineer.Features.ArmorMaximizer;

internal class ArmorMaximizerFeature : Feature<ArmorMaximizerSettings>
{
    internal static readonly ArmorMaximizerFeature Shared = new();

    internal override ArmorMaximizerSettings Settings => Control.settings.ArmorMaximizer;

    internal override void SetupFeatureLoaded()
    {
        // TODO move to per mech change
        if (Settings.LockHeadByDefault)
        {
            ArmorLocationLocker.ToggleLock(ArmorLocation.Head);
        }
    }
}