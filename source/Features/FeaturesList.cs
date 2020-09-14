﻿using MechEngineer.Features.AccuracyEffects;
using MechEngineer.Features.ArmActuators;
using MechEngineer.Features.ArmorStructureChanges;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.AutoFix;
using MechEngineer.Features.ComponentExplosions;
using MechEngineer.Features.CompressFloatieMessages;
using MechEngineer.Features.CriticalEffects;
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
using MechEngineer.Features.OmniSlots;
using MechEngineer.Features.OrderedStatusEffects;
using MechEngineer.Features.OverrideDescriptions;
using MechEngineer.Features.OverrideGhostVFX;
using MechEngineer.Features.OverrideStatTooltips;
using MechEngineer.Features.OverrideTonnage;
using MechEngineer.Features.Performance;
using MechEngineer.Features.PlaceholderEffects;
using MechEngineer.Features.ShutdownInjuryProtection;
using MechEngineer.Features.TagManager;
using MechEngineer.Features.TurretLimitedAmmo;
using MechEngineer.Features.TurretMechComponents;

namespace MechEngineer.Features
{
    internal class FeaturesList
    {
        // order matters, dependencies between "Features" are encoded into the order
        internal static IFeature[] Features = {
            OrderedStatusEffectsFeature.Shared,
            OverrideTonnageFeature.Shared,
            HeatSinkCapacityStatFeature.Shared,
            EngineFeature.Shared,
            MoveMultiplierStatFeature.Shared,
            CompressFloatieMessagesFeature.Shared,
            DamageIgnoreFeature.Shared,
            PlaceholderEffectsFeature.Shared,
            CriticalEffectsFeature.Shared,
            AccuracyEffectsFeature.Shared,
            OverrideDescriptionsFeature.Shared,
            ArmActuatorFeature.Shared,
            DynamicSlotsFeature.Shared,
            ShutdownInjuryProtectionFeature.Shared,
            MechLabSlotsFeature.Shared,
            InvalidInventoryFeature.Shared,
            ComponentExplosionsFeature.Shared,
            ArmorStructureRatioFeature.Shared,
            ArmorStructureChangesFeature.Shared,
            HardpointFixFeature.Shared,
            AutoFixerFeature.Shared,
            GlobalsFeature.Shared,
            TurretMechComponentsFeature.Shared,
            TurretLimitedAmmoFeature.Shared,
            OverrideGhostVFXFeature.Shared,
            OmniSlotsFeature.Shared,
            TagManagerFeature.Shared,
            OverrideStatTooltipsFeature.Shared,
            PerformanceFeature.Shared,

            DebugSaveMechToFileFeature.Shared,
            DebugCycleCombatSoundsFeature.Shared,
            DebugScreenshotMechsFeature.Shared
        };
    }
}
