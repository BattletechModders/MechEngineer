using MechEngineer.Features.AccuracyEffects;
using MechEngineer.Features.ArmorMaximizer;
using MechEngineer.Features.ArmorStructureChanges;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.AutoFix;
using MechEngineer.Features.ComponentExplosions;
using MechEngineer.Features.CompressFloatieMessages;
using MechEngineer.Features.CriticalEffects;
using MechEngineer.Features.CustomCapacities;
using MechEngineer.Features.DamageIgnore;
using MechEngineer.Features.DebugCycleCombatSounds;
using MechEngineer.Features.DebugSaveMechToFile;
using MechEngineer.Features.DebugScreenshotMechs;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Globals;
using MechEngineer.Features.HardpointFix;
using MechEngineer.Features.HeatSinkCapacityStat;
using MechEngineer.Features.InvalidInventory;
using MechEngineer.Features.MechLabSlots;
using MechEngineer.Features.MoveMultiplierStat;
using MechEngineer.Features.OrderedStatusEffects;
using MechEngineer.Features.OverrideDescriptions;
using MechEngineer.Features.OverrideGhostVFX;
using MechEngineer.Features.OverrideStatTooltips;
using MechEngineer.Features.OverrideTonnage;
using MechEngineer.Features.Performance;
using MechEngineer.Features.PlaceholderEffects;
using MechEngineer.Features.ShutdownInjuryProtection;
using MechEngineer.Features.TurretLimitedAmmo;
using MechEngineer.Features.TurretMechComponents;

namespace MechEngineer.Features;

internal static class FeaturesList
{
    // order matters, dependencies between "Features" are encoded into the order
    internal static readonly IFeature[] Features =
    {
        OrderedStatusEffectsFeature.Shared,
        OverrideTonnageFeature.Shared,
        CustomCapacitiesFeature.Shared,
        HeatSinkCapacityStatFeature.Shared,
        EngineFeature.Shared,
        MoveMultiplierStatFeature.Shared,
        CompressFloatieMessagesFeature.Shared,
        DamageIgnoreFeature.Shared,
        PlaceholderEffectsFeature.Shared,
        CriticalEffectsFeature.Shared,
        AccuracyEffectsFeature.Shared,
        OverrideDescriptionsFeature.Shared,
        DynamicSlotsFeature.Shared,
        ShutdownInjuryProtectionFeature.Shared,
        MechLabSlotsFeature.Shared,
        InvalidInventoryFeature.Shared,
        ComponentExplosionsFeature.Shared,
        ArmorStructureRatioFeature.Shared,
        ArmorStructureChangesFeature.Shared,
        ArmorMaximizerFeature.Shared,
        HardpointFixFeature.Shared,
        AutoFixerFeature.Shared,
        GlobalsFeature.Shared,
        TurretMechComponentsFeature.Shared,
        TurretLimitedAmmoFeature.Shared,
        OverrideGhostVFXFeature.Shared,
        OverrideStatTooltipsFeature.Shared,
        PerformanceFeature.Shared,
        DebugSaveMechToFileFeature.Shared,
        DebugCycleCombatSoundsFeature.Shared,
        DebugScreenshotMechsFeature.Shared
    };
}