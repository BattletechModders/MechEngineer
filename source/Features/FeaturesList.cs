using MechEngineer.Features.AccuracyEffects;
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
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Globals;
using MechEngineer.Features.HardpointFix;
using MechEngineer.Features.InvalidInventory;
using MechEngineer.Features.LocationalEffects;
using MechEngineer.Features.MechLabSlots;
using MechEngineer.Features.MoveMultiplierStat;
using MechEngineer.Features.NewSaveFolder;
using MechEngineer.Features.OmniSlots;
using MechEngineer.Features.OverrideDescriptions;
using MechEngineer.Features.OverrideGhostVFX;
using MechEngineer.Features.OverrideTonnage;
using MechEngineer.Features.ShutdownInjuryProtection;
using MechEngineer.Features.TurretLimitedAmmo;
using MechEngineer.Features.TurretMechComponents;

namespace MechEngineer.Features
{
    internal class FeaturesList
    {
        // order matters, dependencies between "Features" are encoded into the order
        internal static IFeature[] Features = {
            OverrideTonnageFeature.Shared,
            EngineFeature.Shared,
            MoveMultiplierStatFeature.Shared,
            CompressFloatieMessagesFeature.Shared,
            DamageIgnoreFeature.Shared,
            LocationalEffectsFeature.Shared,
            CriticalEffectsFeature.Shared,
            AccuracyEffectsFeature.Shared,
            OverrideDescriptionsFeature.Shared,
            ArmActuatorFeature.Shared,
            DynamicSlotsFeature.Shared,
            ShutdownInjuryProtectionFeature.Shared,
            DebugSaveMechToFileFeature.Shared,
            DebugCycleCombatSoundsFeature.Shared,
            NewSaveFolderFeature.Shared,
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
            OmniSlotsFeature.Shared
        };
    }
}
