using MechEngineer.Features.AccuracyEffects;
using MechEngineer.Features.ArmActuators;
using MechEngineer.Features.CompressFloatieMessages;
using MechEngineer.Features.CriticalEffects;
using MechEngineer.Features.DebugCycleCombatSounds;
using MechEngineer.Features.DebugSaveMechToFile;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.LocationalEffects;
using MechEngineer.Features.MoveMultiplierStat;
using MechEngineer.Features.OverrideDescriptions;
using MechEngineer.Features.ShutdownInjuryProtection;

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
            AccuracyEffectsFeature.Shared,
            OverrideDescriptionsFeature.Shared,
            ArmActuatorFeature.Shared,
            DynamicSlotFeature.Shared, // not nicely modularized yet
            ShutdownInjuryProtectionFeature.Shared,
            DebugSaveMechToFileFeature.Shared,
            DebugCycleCombatSoundsFeature.Shared
        };
    }
}
