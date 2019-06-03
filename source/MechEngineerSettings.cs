using BattleTech;
using MechEngineer.Features;
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
        public BaseSettings AccuracyEffects = new BaseSettings();
        public ArmActuatorSettings ArmActuator = new ArmActuatorSettings();
        public BaseSettings ArmorStructureChanges = new BaseSettings();
        public ArmorStructureRatioSettings ArmorStructureRatio = new ArmorStructureRatioSettings();
        public AutoFixerSettings AutoFixer = new AutoFixerSettings();
        public BaseSettings BattleTechLoadFix = new BaseSettings();
        public BaseSettings ComponentExplosions = new BaseSettings();
        public CompressFloatieMessagesSettings CompressFloatieMessages = new CompressFloatieMessagesSettings();
        public CriticalEffectsSettings CriticalEffects = new CriticalEffectsSettings();
        public BaseSettings DebugCycleCombatSounds = new BaseSettings { Enabled = false };
        public BaseSettings DebugSaveMechToFile = new BaseSettings { Enabled = false };
        public DynamicSlotsSettings DynamicSlots = new DynamicSlotsSettings();
        public EngineSettings Engine = new EngineSettings();
        
        public HardpointFixSettings HardpointFix = new HardpointFixSettings();
        public BaseSettings InvalidInventory = new BaseSettings();
        
        public MechLabSlotsSettings MechLabSlots = new MechLabSlotsSettings();
        public BaseSettings MoveMultiplierStat = new BaseSettings();
        public NewSaveFolderSettings NewSaveFolder = new NewSaveFolderSettings { Enabled =  false };
        public OverrideDescriptionsSettings OverrideDescriptions = new OverrideDescriptionsSettings();
        public OverrideGhostVFXSettings OverrideGhostVFX = new OverrideGhostVFXSettings();
        public OverrideTonnageSettings OverrideTonnage = new OverrideTonnageSettings();
        public ShutdownInjuryProtectionSettings ShutdownInjuryProtection = new ShutdownInjuryProtectionSettings();
        public BaseSettings TurretLimitedAmmo = new BaseSettings();
        public BaseSettings TurretMechComponents = new BaseSettings();
    }
}
