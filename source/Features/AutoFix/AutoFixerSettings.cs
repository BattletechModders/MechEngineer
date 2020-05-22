using BattleTech;

namespace MechEngineer.Features.AutoFix
{
    internal class AutoFixerSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Fixes up mechs, chassis and components to adhere to CBT rules and defaults. Done programmatically to be compatible to new mechs in the future.";

        // mech defs that should use full free tonnage when autofixing (workaround that weight reduction items are not considered)
        public string[] MechDefAutoFixAgainstMaxFreeTonnage = {"mechdef_atlas_AS7-D-HT", "mechdef_blackknight_BL-6b-KNT", "mechdef_highlander_HGN-732b"};
        // how much more tonnage is allowed to be used for adding engines etc.. then what was given by reducing initial tonnage (can never exceed max tonnage anyway)
        public float MechDefAutoFixInitialTonnageDiffThreshold = 0.4f; // less than 0.5 so no new engine can be put on

        public bool MechDefEngine = true; // adds missing engine and removes too many jump jets
        public string MechDefCoolingDef = "emod_kit_shs";
        public string MechDefHeatBlockDef = "emod_engine_cooling";
        public string MechDefCoreDummy = "emod_engine_dummy";

        public IdentityHelper GyroCategorizer = new IdentityHelper
        {
            AllowedLocations = ChassisLocations.CenterTorso, // optional if category is properly setup
            ComponentType = ComponentType.Upgrade, // optional if category is properly setup
            Prefix = "Gear_Gyro_", // optional if category is properly setup
            CategoryId = "Gyro", // required
            AutoAddCategoryIdIfMissing = true // adds category id to items matched by optional filters
        };
        public SlotChange GyroSlotChange = new SlotChange { From = 3, By = 1 };

        public IdentityHelper CockpitCategorizer = new IdentityHelper
        {
            AllowedLocations = ChassisLocations.Head,
            ComponentType = ComponentType.Upgrade,
            Prefix = "Gear_Cockpit_",
            CategoryId = "Cockpit",
            AutoAddCategoryIdIfMissing = true
        };
        public TonnageChange CockpitTonnageChange = new TonnageChange { From = 0, By = 3 };
        public SlotChange CockpitSlotChange = new SlotChange { From = 1, By = 0 };

        public IdentityHelper LegUpgradesCategorizer = new IdentityHelper
        {
            AllowedLocations = ChassisLocations.Legs,
            ComponentType = ComponentType.Upgrade,
            Prefix = null, //"Gear_Actuator_";
            CategoryId = "LegFootActuator",
            AutoAddCategoryIdIfMissing = true
        };
        public SlotChange LegUpgradesSlotChange = new SlotChange { From = 3, By = -1, FromIsMin = true, NewMin = 1 };

        public ChassisSlotsChange[] ChassisDefSlotsChanges = {
            // vanilla mechs
            new ChassisSlotsChange
            {
                Location = ChassisLocations.LeftTorso,
                Change = new SlotChange {From = 10, By = 2}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.RightTorso,
                Change = new SlotChange {From = 10, By = 2}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.LeftLeg,
                Change = new SlotChange {From = 4, By = 2}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.RightLeg,
                Change = new SlotChange {From = 4, By = 2}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.Head,
                Change = new SlotChange {From = 1, By = 5}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.CenterTorso,
                Change = new SlotChange {From = 4, By = 11}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.LeftArm,
                Change = new SlotChange {From = 8, By = 4}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.RightArm,
                Change = new SlotChange {From = 8, By = 4}
            },
            // old ME values
            new ChassisSlotsChange
            {
                Location = ChassisLocations.LeftLeg,
                Change = new SlotChange {From = 2, By = 4}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.RightLeg,
                Change = new SlotChange {From = 2, By = 4}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.Head,
                Change = new SlotChange {From = 3, By = 3}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.LeftArm,
                Change = new SlotChange {From = 11, By = 1}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.RightArm,
                Change = new SlotChange {From = 11, By = 1}
            },
        };

        public bool ChassisDefInitialTonnage = true;
        public float ChassisDefInitialToTotalTonnageFactor = 0.1f; // 10% structure weight
        public bool ChassisDefMaxJumpjets = true;
        public int ChassisDefMaxJumpjetsCount = 8;
        public int ChassisDefMaxJumpjetsRating = 400;

        public bool AutoFixWeaponDefSplitting = true;
        public int AutoFixWeaponDefSplittingLargerThan = 7;

        public WeaponDefChange[] AutoFixWeaponDefSlotsChanges = {
            new WeaponDefChange
            {
                Type = WeaponSubType.AC5,
                SlotChange = new SlotChange {From = 2, By = 2}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.AC10,
                SlotChange = new SlotChange {From = 3, By = 4}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.AC20,
                SlotChange = new SlotChange {From = 4, By = 6}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.Gauss,
                SlotChange = new SlotChange {From = 5, By = 2}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.LRM20,
                SlotChange = new SlotChange {From = 4, By = 1}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.TAG,
                SlotChange = new SlotChange {From = 3, By = -2}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.LB2X,
                SlotChange = new SlotChange {From = 1, By = 3},
                TonnageChange = new TonnageChange {From = 5, By = 1}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.LB5X,
                SlotChange = new SlotChange {From = 2, By = 3}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.LB10X,
                SlotChange = new SlotChange {From = 4, By = 2}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.LB20X,
                SlotChange = new SlotChange {From = 6, By = 5}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.UAC2,
                SlotChange = new SlotChange {From = 1, By = 2}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.UAC5,
                SlotChange = new SlotChange {From = 2, By = 3}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.UAC10,
                SlotChange = new SlotChange {From = 3, By = 4}
            },
            new WeaponDefChange
            {
                Type = WeaponSubType.UAC20,
                SlotChange = new SlotChange {From = 4, By = 6}
            }
        };
    }
}