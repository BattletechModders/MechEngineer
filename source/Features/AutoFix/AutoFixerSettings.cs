using BattleTech;
using MechEngineer.Features.MechLabSlots;

namespace MechEngineer.Features.AutoFix;

internal class AutoFixerSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Fixes up mechs, chassis and components to adhere to CBT rules and defaults. Done programmatically to be compatible to new mechs in the future.";

    public bool MechDefEngine = true;
    public string[] MechTagsAutoFixEnabled = {"unit_release"};
    public string MechDefHeatBlockDef = "emod_engine_cooling";
    public string MechDefCoreDummy = "emod_engine_dummy";

    public string[] IgnoreAutofixTags = {"ignore_autofix", "noautofix"};
    public string IgnoreAutofixTagsDescription => "These tags on a MechDef, ChassisDef or ComponentDef make sure that the respective object is being ignored by the autofixer";

    public bool InternalizeHeatSinksOnValidEngines = true;
    public string InternalizeHeatSinksOnValidEnginesDescription => "Converts external heat sinks to internal heat sinks (critical slots -> engine) even for valid mech layouts. Useful when autofixing vanilla mechs, conflicts with modded loadouts.";

    public MechLocationNamingTemplateByTags[] MechLocationNamingTemplates =
    {
        new()
        {
            Tags = new[] {"unit_vtol"},
            Template = new ChassisLocationNaming
            {
                Names = new[]
                {
                    new ChassisLocationNaming.LocationName
                    {
                        Location = ChassisLocations.LeftArm,
                        Label = "FRONT",
                        ShortLabel = "F"
                    },
                    new ChassisLocationNaming.LocationName
                    {
                        Location = ChassisLocations.RightArm,
                        Label = "REAR",
                        ShortLabel = "R"
                    },
                    new ChassisLocationNaming.LocationName
                    {
                        Location = ChassisLocations.LeftLeg,
                        Label = "LEFT SIDE",
                        ShortLabel = "LS"
                    },
                    new ChassisLocationNaming.LocationName
                    {
                        Location = ChassisLocations.RightLeg,
                        Label = "RIGHT SIDE",
                        ShortLabel = "RS"
                    },
                    new ChassisLocationNaming.LocationName
                    {
                        Location = ChassisLocations.Head,
                        Label = "ROTOR",
                        ShortLabel = "RO"
                    }
                }
            }
        },
        new()
        {
            Tags = new[] {"fake_vehicle_chassis"},
            Template = new ChassisLocationNaming
            {
                Names = new[]
                {
                    new ChassisLocationNaming.LocationName
                    {
                        Location = ChassisLocations.LeftArm,
                        Label = "FRONT",
                        ShortLabel = "F"
                    },
                    new ChassisLocationNaming.LocationName
                    {
                        Location = ChassisLocations.RightArm,
                        Label = "REAR",
                        ShortLabel = "R"
                    },
                    new ChassisLocationNaming.LocationName
                    {
                        Location = ChassisLocations.LeftLeg,
                        Label = "LEFT SIDE",
                        ShortLabel = "LS"
                    },
                    new ChassisLocationNaming.LocationName
                    {
                        Location = ChassisLocations.RightLeg,
                        Label = "RIGHT SIDE",
                        ShortLabel = "RS"
                    },
                    new ChassisLocationNaming.LocationName
                    {
                        Location = ChassisLocations.Head,
                        Label = "TURRET",
                        ShortLabel = "T"
                    }
                }
            }
        }
    };
    public string MechLocationNamingTemplatesDescription => $"If any of the listed tags are found, and the chassis has no {nameof(ChassisLocationNaming)} component, add the associated {nameof(ChassisLocationNaming)} component";


    public class MechLocationNamingTemplateByTags
    {
        public string[] Tags = { };
        public ChassisLocationNaming Template = new();
    }

    public IdentityHelper GyroCategorizer = new()
    {
        AllowedLocations = ChassisLocations.CenterTorso, // optional if category is properly setup
        ComponentType = ComponentType.Upgrade, // optional if category is properly setup
        Prefix = "Gear_Gyro_", // optional if category is properly setup
        CategoryId = "Gyro", // required
        AutoAddCategoryIdIfMissing = true // adds category id to items matched by optional filters
    };
    public SlotChange GyroSlotChange = new() {From = 3, By = 1};

    public IdentityHelper CockpitCategorizer = new()
    {
        AllowedLocations = ChassisLocations.Head,
        ComponentType = ComponentType.Upgrade,
        Prefix = "Gear_Cockpit_Ceres_",
        CategoryId = "Cockpit",
        AutoAddCategoryIdIfMissing = true
    };
    public TonnageChange CockpitTonnageChange = new() {From = 0, By = 3};
    public SlotChange CockpitSlotChange = new() {From = 1, By = 0};

    public IdentityHelper SensorsACategorizer = new()
    {
        AllowedLocations = ChassisLocations.Head,
        ComponentType = ComponentType.Upgrade,
        Prefix = "Gear_Cockpit_",
        CategoryId = "SensorsA",
        AutoAddCategoryIdIfMissing = true
    };

    public IdentityHelper SensorsBCategorizer = new()
    {
        AllowedLocations = ChassisLocations.All,
        ComponentType = ComponentType.Upgrade,
        Prefix = "Gear_TargetingTrackingSystem_",
        CategoryId = "SensorsB",
        AutoAddCategoryIdIfMissing = true
    };
    public SlotChange SensorsBSlotChange = new() {From = 2, By = -1};

    public IdentityHelper LegUpgradesCategorizer = new()
    {
        AllowedLocations = ChassisLocations.Legs,
        ComponentType = ComponentType.Upgrade,
        Prefix = null, //"Gear_Actuator_";
        CategoryId = "LegFootActuator",
        AutoAddCategoryIdIfMissing = true
    };
    public SlotChange LegUpgradesSlotChange = new() {From = 3, By = -1, FromIsMin = true, NewMin = 1};

    public ChassisSlotsChange[] ChassisDefSlotsChanges =
    {
        // vanilla mechs
        new()
        {
            Location = ChassisLocations.LeftTorso,
            Change = new SlotChange {From = 10, By = 2}
        },
        new()
        {
            Location = ChassisLocations.RightTorso,
            Change = new SlotChange {From = 10, By = 2}
        },
        new()
        {
            Location = ChassisLocations.LeftLeg,
            Change = new SlotChange {From = 4, By = 2}
        },
        new()
        {
            Location = ChassisLocations.RightLeg,
            Change = new SlotChange {From = 4, By = 2}
        },
        new()
        {
            Location = ChassisLocations.Head,
            Change = new SlotChange {From = 1, By = 5}
        },
        new()
        {
            Location = ChassisLocations.CenterTorso,
            Change = new SlotChange {From = 4, By = 8}
        },
        new()
        {
            Location = ChassisLocations.LeftArm,
            Change = new SlotChange {From = 8, By = 4}
        },
        new()
        {
            Location = ChassisLocations.RightArm,
            Change = new SlotChange {From = 8, By = 4}
        },
        // old ME values
        new()
        {
            Location = ChassisLocations.LeftLeg,
            Change = new SlotChange {From = 2, By = 4}
        },
        new()
        {
            Location = ChassisLocations.RightLeg,
            Change = new SlotChange {From = 2, By = 4}
        },
        new()
        {
            Location = ChassisLocations.Head,
            Change = new SlotChange {From = 3, By = 3}
        },
        new()
        {
            Location = ChassisLocations.LeftArm,
            Change = new SlotChange {From = 11, By = 1}
        },
        new()
        {
            Location = ChassisLocations.RightArm,
            Change = new SlotChange {From = 11, By = 1}
        }
    };

    public bool ChassisDefInitialTonnage = true;
    public float ChassisDefInitialToTotalTonnageFactor = 0.1f; // 10% structure weight
    public bool ChassisDefArmorStructureRatio = true;
    public bool ChassisDefMaxJumpjets = true;
    public int ChassisDefMaxJumpjetsCount = 8;
    public int ChassisDefMaxJumpjetsRating = 400;

    public bool AutoFixWeaponDefSplitting = true;
    public int AutoFixWeaponDefSplittingLargerThan = 7;
    public int AutoFixWeaponDefSplittingFixedSize = 7; // use 1 for CBT default

    public DynamicSlots.DynamicSlots AutoFixWeaponDefSplittingDynamicSlotTemplate =
        new() {InnerAdjacentOnly = true};

    public WeaponDefChange[] AutoFixWeaponDefSlotsChanges =
    {
        new()
        {
            Type = WeaponSubType.AC5,
            SlotChange = new SlotChange {From = 2, By = 2}
        },
        new()
        {
            Type = WeaponSubType.AC10,
            SlotChange = new SlotChange {From = 3, By = 4}
        },
        new()
        {
            Type = WeaponSubType.AC20,
            SlotChange = new SlotChange {From = 4, By = 6}
        },
        new()
        {
            Type = WeaponSubType.Gauss,
            SlotChange = new SlotChange {From = 4, By = 2, FromIsMin = true}
        },
        new()
        {
            Type = WeaponSubType.LRM20,
            SlotChange = new SlotChange {From = 4, By = 1}
        },
        new()
        {
            Type = WeaponSubType.TAG,
            SlotChange = new SlotChange {From = 3, By = -2}
        },
        new()
        {
            Type = WeaponSubType.LB2X,
            SlotChange = new SlotChange {From = 1, By = 3},
            TonnageChange = new TonnageChange {From = 5, By = 1}
        },
        new()
        {
            Type = WeaponSubType.LB5X,
            SlotChange = new SlotChange {From = 2, By = 3}
        },
        new()
        {
            Type = WeaponSubType.LB10X,
            SlotChange = new SlotChange {From = 4, By = 2}
        },
        new()
        {
            Type = WeaponSubType.LB20X,
            SlotChange = new SlotChange {From = 6, By = 5}
        },
        new()
        {
            Type = WeaponSubType.UAC2,
            SlotChange = new SlotChange {From = 1, By = 2}
        },
        new()
        {
            Type = WeaponSubType.UAC5,
            SlotChange = new SlotChange {From = 2, By = 3}
        },
        new()
        {
            Type = WeaponSubType.UAC10,
            SlotChange = new SlotChange {From = 3, By = 4}
        },
        new()
        {
            Type = WeaponSubType.UAC20,
            SlotChange = new SlotChange {From = 4, By = 6}
        }
    };
}