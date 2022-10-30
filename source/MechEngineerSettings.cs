using MechEngineer.Features.AccuracyEffects;
using MechEngineer.Features.ArmorMaximizer;
using MechEngineer.Features.ArmorStructureChanges;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.AutoFix;
using MechEngineer.Features.BetterLog;
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
using MechEngineer.Features.TagManager;
using MechEngineer.Features.TurretLimitedAmmo;
using MechEngineer.Features.TurretMechComponents;
using MechEngineer.Misc;

namespace MechEngineer;

[UsedBy(User.FastJson)]
internal class MechEngineerSettings
{
    public bool GeneratedSettingsFilesReadonly = true;
    public string GeneratedSettingsFilesReadonlyDescription => "If true, generated last and default settings files are set to readonly, to indicate that those are not intended to be edited.";

    public BetterLogSettings BetterLog = new();

    public AccuracyEffectsSettings AccuracyEffects = new();
    public ArmorStructureChangesSettings ArmorStructureChanges = new();
    public ArmorStructureRatioSettings ArmorStructureRatio = new();
    public ArmorMaximizerSettings ArmorMaximizer = new();
    public AutoFixerSettings AutoFixer = new();
    public ComponentExplosionsSettings ComponentExplosions = new();
    public CompressFloatieMessagesSettings CompressFloatieMessages = new();
    public CriticalEffectsSettings CriticalEffects = new();
    public CustomCapacitiesSettings CustomCapacities = new();
    public DamageIgnoreSettings DamageIgnore = new();
    public DynamicSlotsSettings DynamicSlots = new();
    public EngineSettings Engine = new();
    public GlobalsSettings Globals = new();
    public HardpointFixSettings HardpointFix = new();
    public HeatSinkCapacityStatSettings HeatSinkCapacityStat = new();
    public InvalidInventorySettings InvalidInventory = new();
    public MechLabSlotsSettings MechLabSlots = new();
    public MoveMultiplierStatSettings MoveMultiplierStat = new();
    public OrderedStatusEffectsSettings OrderedStatusEffects = new();
    public OverrideDescriptionsSettings OverrideDescriptions = new();
    public OverrideGhostVFXSettings OverrideGhostVFX = new();
    public OverrideStatTooltipsSettings OverrideStatTooltips = new();
    public OverrideTonnageSettings OverrideTonnage = new();
    public PlaceholderEffectsSettings PlaceholderEffects = new();
    public ShutdownInjuryProtectionSettings ShutdownInjuryProtection = new();
    public TurretLimitedAmmoSettings TurretLimitedAmmo = new();
    public TurretMechComponentSettings TurretMechComponents = new();
    public TagManagerSettings TagManager = new();

    public PerformanceSettings Performance = new();

    public DebugCycleCombatSoundsSettings DebugCycleCombatSounds = new();
    public DebugSaveMechToFileSettings DebugSaveMechToFile = new();
    public DebugScreenshotMechsSettings DebugScreenshotMechs = new();
}