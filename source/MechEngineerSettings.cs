using BattleTech;
using MechEngineer.Features.AccuracyEffects;
using MechEngineer.Features.ArmActuators;
using MechEngineer.Features.ArmorStructureChanges;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.AutoFix;
using MechEngineer.Features.BattleTechLoadFix;
using MechEngineer.Features.ComponentExplosions;
using MechEngineer.Features.CompressFloatieMessages;
using MechEngineer.Features.CriticalEffects;
using MechEngineer.Features.DebugCycleCombatSounds;
using MechEngineer.Features.DebugSaveMechToFile;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.Engines;
using MechEngineer.Features.HardpointFix;
using MechEngineer.Features.InvalidInventory;
using MechEngineer.Features.MechLabSlots;
using MechEngineer.Features.MoveMultiplierStat;
using MechEngineer.Features.NewSaveFolder;
using MechEngineer.Features.OverrideDescriptions;
using MechEngineer.Features.OverrideGhostVFX;
using MechEngineer.Features.OverrideTonnage;
using MechEngineer.Features.ShutdownInjuryProtection;
using MechEngineer.Features.TurretLimitedAmmo;
using MechEngineer.Features.TurretMechComponents;

namespace MechEngineer
{
    public class AddHelper
    {
        public string ComponentDefId { get; set; }
        public ChassisLocations ChassisLocation { get; set; }
        public ComponentType ComponentType { get; set; }
    }

    internal class MechEngineerSettings
    {
        public NewSaveFolderFeature.Settings NewSaveFolder = new NewSaveFolderFeature.Settings();
        public OverrideGhostVFXFeature.Settings OverrideGhostVFX = new OverrideGhostVFXFeature.Settings();
        public HardpointFixFeature.Settings HardpointFix = new HardpointFixFeature.Settings();
        public ArmActuatorFeature.Settings ArmActuator = new ArmActuatorFeature.Settings();
        public ShutdownInjuryProtectionFeature.Settings ShutdownInjuryProtection = new ShutdownInjuryProtectionFeature.Settings();
        public ArmorStructureRatioFeature.Settings ArmorStructureRatio = new ArmorStructureRatioFeature.Settings();
        public DebugSaveMechToFileFeature.Settings DebugSaveMechToFile = new DebugSaveMechToFileFeature.Settings();
        public OverrideTonnageFeature.Settings OverrideTonnage = new OverrideTonnageFeature.Settings();
        public CompressFloatieMessagesFeature.Settings CompressFloatieMessages = new CompressFloatieMessagesFeature.Settings();
        public MoveMultiplierStatFeature.Settings MoveMultiplierStat = new MoveMultiplierStatFeature.Settings();
        public TurretLimitedAmmoFeature.Settings TurretLimitedAmmo = new TurretLimitedAmmoFeature.Settings();
        public DebugCycleCombatSoundsFeature.Settings DebugCycleCombatSounds = new DebugCycleCombatSoundsFeature.Settings();
        public EngineFeature.Settings Engine = new EngineFeature.Settings();
        public MechLabSlotsFeature.Settings MechLabSlots = new MechLabSlotsFeature.Settings();
        public DynamicSlotsFeature.Settings DynamicSlots = new DynamicSlotsFeature.Settings();
        public AccuracyEffectsFeature.Settings AccuracyEffects = new AccuracyEffectsFeature.Settings();
        public OverrideDescriptionsFeature.Settings OverrideDescriptions = new OverrideDescriptionsFeature.Settings();
        public CriticalEffectsFeature.Settings CriticalEffects = new CriticalEffectsFeature.Settings();
        public AutoFixerFeature.Settings AutoFixer = new AutoFixerFeature.Settings();
        public ArmorStructureChangesFeature.Settings ArmorStructureChanges = new ArmorStructureChangesFeature.Settings();
        public BattleTechLoadFixFeature.Settings BattleTechLoadFix = new BattleTechLoadFixFeature.Settings();
        public ComponentExplosionsFeature.Settings ComponentExplosions = new ComponentExplosionsFeature.Settings();
        public TurretMechComponentsFeature.Settings TurretMechComponents = new TurretMechComponentsFeature.Settings();
        public InvalidInventoryFeature.Settings InvalidInventory = new InvalidInventoryFeature.Settings();
    }
}
