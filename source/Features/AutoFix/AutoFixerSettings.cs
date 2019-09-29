using BattleTech;

namespace MechEngineer.Features.AutoFix
{
    internal class AutoFixerSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Fixes up mechs, chassis and components to adhere to CBT rules and defaults. Done programmatically to be compatible to new mechs in the future.";

        public string[] MechDefSkip = { }; // mech defs to skip for AutoFixMechDef*
        public string[] MechDefTagsSkip = {"noautofix"};

        public bool MechDefEngine = true; // adds missing engine and removes too many jump jets
        public string MechDefCoolingDef = "emod_kit_shs";
        public string MechDefHeatBlockDef = "emod_engine_cooling";
        public string MechDefCoreDummy = "emod_engine_dummy";
            
        public string[] UpgradeDefSkip = { }; // upgrades to not autofix
        public string[] UpgradeDefTagsSkip = {"noautofix"};

        public IdentityHelper GyroCategorizer = new IdentityHelper
        {
            AllowedLocations = ChassisLocations.CenterTorso, // optional if category is properly setup
            ComponentType = ComponentType.Upgrade, // optional if category is properly setup
            Prefix = "Gear_Gyro_", // optional if category is properly setup
            CategoryId = "Gyro", // required
            AutoAddCategoryIdIfMissing = true // adds category id to items matched by optional filters
        };
        public ValueChange<int> GyroSlotChange = new ValueChange<int> { From = 3, By = 1 };

        public IdentityHelper CockpitCategorizer = new IdentityHelper
        {
            AllowedLocations = ChassisLocations.Head,
            ComponentType = ComponentType.Upgrade,
            Prefix = "Gear_Cockpit_",
            CategoryId = "Cockpit",
            AutoAddCategoryIdIfMissing = true
        };
        public ValueChange<float> CockpitTonnageChange = new ValueChange<float> { From = 0, By = 3 };
        public ValueChange<int> CockpitSlotChange = new ValueChange<int> { From = 1, By = 0 };

        public IdentityHelper LegUpgradesCategorizer = new IdentityHelper
        {
            AllowedLocations = ChassisLocations.Legs,
            ComponentType = ComponentType.Upgrade,
            Prefix = null, //"Gear_Actuator_";
            CategoryId = "LegFootActuator",
            AutoAddCategoryIdIfMissing = true
        };
        public ValueChange<int> LegUpgradesSlotChange = new ValueChange<int> { From = 3, By = -1, FromIsMin = true, NewMin = 1 };

        public string[] ChassisDefSkip = { };
        public string[] ChassisDefTagsSkip = {"noautofix"};

        public ChassisSlotsChange[] ChassisDefSlotsChanges = {
            // vanilla mechs
            new ChassisSlotsChange
            {
                Location = ChassisLocations.LeftTorso,
                Change = new ValueChange<int> {From = 10, By = 2}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.RightTorso,
                Change = new ValueChange<int> {From = 10, By = 2}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.LeftLeg,
                Change = new ValueChange<int> {From = 4, By = 2}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.RightLeg,
                Change = new ValueChange<int> {From = 4, By = 2}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.Head,
                Change = new ValueChange<int> {From = 1, By = 5}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.CenterTorso,
                Change = new ValueChange<int> {From = 4, By = 11}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.LeftArm,
                Change = new ValueChange<int> {From = 8, By = 4}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.RightArm,
                Change = new ValueChange<int> {From = 8, By = 4}
            },
            // old ME values
            new ChassisSlotsChange
            {
                Location = ChassisLocations.LeftLeg,
                Change = new ValueChange<int> {From = 2, By = 4}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.RightLeg,
                Change = new ValueChange<int> {From = 2, By = 4}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.Head,
                Change = new ValueChange<int> {From = 3, By = 3}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.LeftArm,
                Change = new ValueChange<int> {From = 11, By = 1}
            },
            new ChassisSlotsChange
            {
                Location = ChassisLocations.RightArm,
                Change = new ValueChange<int> {From = 11, By = 1}
            },
        };

        public bool ChassisDefInitialTonnage = true;
        public float ChassisDefInitialToTotalTonnageFactor = 0.1f; // 10% structure weight
        public bool ChassisDefMaxJumpjets = true;
        public int ChassisDefMaxJumpjetsCount = 8;
        public int ChassisDefMaxJumpjetsRating = 400;
        
        public string[] WeaponDefTagsSkip = {"noautofix"};
        public WeaponSlotChange[] AutoFixWeaponDefSlotsChanges = {
            new WeaponSlotChange
            {
                Type = WeaponSubType.AC5,
                Change = new ValueChange<int> {From = 2, By = 2}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.AC10,
                Change = new ValueChange<int> {From = 3, By = 4}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.AC20,
                Change = new ValueChange<int> {From = 4, By = 6}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.Gauss,
                Change = new ValueChange<int> {From = 5, By = 2}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.LRM20,
                Change = new ValueChange<int> {From = 4, By = 1}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.TAG,
                Change = new ValueChange<int> {From = 3, By = -2}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.LB2X,
                Change = new ValueChange<int> {From = 1, By = 3}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.LB5X,
                Change = new ValueChange<int> {From = 2, By = 3}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.LB10X,
                Change = new ValueChange<int> {From = 4, By = 2}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.LB20X,
                Change = new ValueChange<int> {From = 6, By = 5}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.UAC2,
                Change = new ValueChange<int> {From = 1, By = 2}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.UAC5,
                Change = new ValueChange<int> {From = 2, By = 3}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.UAC10,
                Change = new ValueChange<int> {From = 3, By = 4}
            },
            new WeaponSlotChange
            {
                Type = WeaponSubType.UAC20,
                Change = new ValueChange<int> {From = 4, By = 6}
            }
        };
    }
}