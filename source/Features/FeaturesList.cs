using MechEngineer.Features.AccuracyEffects;
using MechEngineer.Features.CompressFloatieMessages;
using MechEngineer.Features.CriticalEffects;
using MechEngineer.Features.LocationalEffects;
using MechEngineer.Features.MoveMultiplierStat;

namespace MechEngineer.Features
{
    internal class FeaturesList
    {
        // order matters, dependencies between "Features" are encoded into the order
        internal static Feature[] Features = {
            MoveMultiplierStatFeature.Shared,
            CompressFloatieMessagesFeature.Shared,
            LocationalEffectsFeature.Shared,
            CriticalEffectsFeature.Shared,
            AccuracyEffectsFeature.Shared
        };
    }
}
