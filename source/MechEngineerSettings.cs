using BattleTech;
using MechEngineer.Features.AccuracyEffects;
using MechEngineer.Features.ArmActuators;
using MechEngineer.Features.ArmorStructureChanges;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.AutoFix;
using MechEngineer.Features.BetterLog;
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
        public bool GeneratedSettingsFilesReadonly = true;
        public string GeneratedSettingsFilesReadonlyDescription => "If true, generated last and default settings files are set to readonly, to indicate that those are not intended to be edited.";

        public BetterLogSettings BetterLog = new BetterLogSettings();

        public AccuracyEffectsSettings AccuracyEffects = new AccuracyEffectsSettings();
        public ArmActuatorSettings ArmActuator = new ArmActuatorSettings();
        public ArmorStructureChangesSettings ArmorStructureChanges = new ArmorStructureChangesSettings();
        public ArmorStructureRatioSettings ArmorStructureRatio = new ArmorStructureRatioSettings();
        public AutoFixerSettings AutoFixer = new AutoFixerSettings();
        public ComponentExplosionsSettings ComponentExplosions = new ComponentExplosionsSettings();
        public CompressFloatieMessagesSettings CompressFloatieMessages = new CompressFloatieMessagesSettings();
        public CriticalEffectsSettings CriticalEffects = new CriticalEffectsSettings();
        public DamageIgnoreSettings DamageIgnore = new DamageIgnoreSettings();
        public DynamicSlotsSettings DynamicSlots = new DynamicSlotsSettings();
        public EngineSettings Engine = new EngineSettings();
        public GlobalsSettings Globals = new GlobalsSettings();
        public HardpointFixSettings HardpointFix = new HardpointFixSettings();
        public HeatSinkCapacityStatSettings HeatSinkCapacityStat = new HeatSinkCapacityStatSettings();
        public InvalidInventorySettings InvalidInventory = new InvalidInventorySettings();
        public MechLabSlotsSettings MechLabSlots = new MechLabSlotsSettings();
        public MoveMultiplierStatSettings MoveMultiplierStat = new MoveMultiplierStatSettings();
        public OrderedStatusEffectsSettings OrderedStatusEffects = new OrderedStatusEffectsSettings();
        public OverrideDescriptionsSettings OverrideDescriptions = new OverrideDescriptionsSettings();
        public OverrideGhostVFXSettings OverrideGhostVFX = new OverrideGhostVFXSettings();
        public OverrideStatTooltipsSettings OverrideStatTooltips = new OverrideStatTooltipsSettings();
        public OverrideTonnageSettings OverrideTonnage = new OverrideTonnageSettings();
        public PlaceholderEffectsSettings PlaceholderEffects = new PlaceholderEffectsSettings();
        public ShutdownInjuryProtectionSettings ShutdownInjuryProtection = new ShutdownInjuryProtectionSettings();
        public TurretLimitedAmmoSettings TurretLimitedAmmo = new TurretLimitedAmmoSettings();
        public TurretMechComponentSettings TurretMechComponents = new TurretMechComponentSettings();
        public TagManagerSettings TagManager = new TagManagerSettings();

        public PerformanceSettings Performance = new PerformanceSettings();
        
        public DebugCycleCombatSoundsSettings DebugCycleCombatSounds = new DebugCycleCombatSoundsSettings();
        public DebugSaveMechToFileSettings DebugSaveMechToFile = new DebugSaveMechToFileSettings();
        public DebugScreenshotMechsSettings DebugScreenshotMechs = new DebugScreenshotMechsSettings();
    }
}
