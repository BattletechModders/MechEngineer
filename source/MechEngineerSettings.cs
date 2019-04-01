using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using HBS.Logging;

namespace MechEngineer
{
    public class AddHelper
    {
        public string ComponentDefId { get; set; }
        public ChassisLocations ChassisLocation { get; set; }
        public ComponentType ComponentType { get; set; }
    }

    public class MechEngineerSettings
    {
        #region misc

        public bool HeatDamageInjuryEnabled = true;
        public bool ShutdownInjuryEnabled = true;

        public int MinimumHeatSinksOnMech = 10; // minimum heatsinks a mech requires
        public bool EnforceRulesForAdditionalInternalHeatSinks = true; // can't have those juicy ++ cooling systems with smaller fusion cores than the rules allow it
        public bool AllowMixingHeatSinkTypes = false; // only useful for patchwork like behavior
        public float FractionalAccountingPrecision = 0.5f; // change to 0.001 for kg fractional accounting precision

        public bool ArmorStructureRatioEnforcement = true;
        public string[] ArmorStructureRatioEnforcementSkipMechDefs = { }; // mech defs to skip

        public bool SaveMechDefOnMechLabConfirm = false;

        public string DefaultEngineHeatSinkId = "Gear_HeatSink_Generic_Standard"; // default heat sink type for engines without a kit

        public int EngineMissingFallbackHeatSinkCapacity = 30; // for stuff that wasn't auto fixed and still missing an engine, use a fallback

        // MWO does not allow to drop if that would mean to go overweight
        // battletech allows overweight, to stay consistent so we also allow overspace usage by default
        // set to true to switch to MWO style
        public bool DynamicSlotsValidateDropEnabled = false;

        public float? ArmorRoundingPrecision = null; // default is ARMOR_PER_STEP * TONNAGE_PER_ARMOR_POINT

        public bool MechLabGeneralWidgetEnabled => MechLabGeneralSlots > 0;
        public int MechLabGeneralSlots = 3;
        public int MechLabArmTopPadding = 120;

        #endregion

        #region bonus descriptions

        public string BonusDescriptionsElementTemplate = " <indent=10%><line-indent=-5%><line-height=65%>{{element}}</line-height></line-indent></indent>\r\n";
        public string BonusDescriptionsDescriptionTemplate = "Traits:<b><color=#F79B26FF>\r\n{{elements}}</color></b>\r\n{{originalDescription}}";
        public string CriticalEffectsDescriptionTemplate = "Critical Effects:<b><color=#F79B26FF>\r\n{{elements}}</color></b>\r\n{{originalDescription}}";

        #endregion

        #region logging

        public class LoggerLogLevel
        {
            public string Name;
            public LogLevel Level;
        }
        public LoggerLogLevel[] LogLevels = {
            new LoggerLogLevel
            {
                Name = "MechEngineer",
                Level = LogLevel.Debug
            }
        };

        #endregion

        #region auto fixes

        public string[] AutoFixMechDefSkip = { }; // mech defs to skip for AutoFixMechDef*
        public string[] AutoFixUpgradeDefSkip =
        {
            "Gear_Cockpit_SLDF_Custom",
            "Gear_Cockpit_Generic_Small",
            "Gear_Cockpit_Generic_Standard",
            "Gear_Cockpit_LifeSupportA_Standard",
            "Gear_Cockpit_LifeSupportB_Standard",
            "Gear_Cockpit_SensorsA_Standard",
            "Gear_Cockpit_SensorsB_Standard",
            "Gear_Gyro_Generic_Standard",
            "emod_arm_part_shoulder",
            "emod_arm_part_upper",
            "emod_arm_part_lower",
            "emod_arm_part_hand",
            "emod_leg_hip",
            "emod_leg_upper",
            "emod_leg_lower",
            "emod_leg_foot",
        }; // upgrades to not autofix

        public bool AutoFixMechDefEngine = true; // adds missing engine and removes too many jump jets
        public string AutoFixMechDefCoolingDef = "emod_kit_shs";
        public string AutoFixMechDefHeatBlockDef = "emod_engine_cooling";
        public string AutoFixMechDefCoreDummy = "emod_engine_dummy";

        public IdentityHelper AutoFixGyroCategorizer = new IdentityHelper
        {
            AllowedLocations = ChassisLocations.CenterTorso, // optional if category is properly setup
            ComponentType = ComponentType.Upgrade, // optional if category is properly setup
            Prefix = "Gear_Gyro_", // optional if category is properly setup
            CategoryId = "Gyro", // required
            AutoAddCategoryIdIfMissing = true // adds category id to items matched by optional filters
        };
        public ValueChange<int> AutoFixGyroSlotChange = new ValueChange<int> {From = 3, By = 1};

        public IdentityHelper AutoFixCockpitCategorizer = new IdentityHelper
        {
            AllowedLocations = ChassisLocations.Head,
            ComponentType = ComponentType.Upgrade,
            Prefix = "Gear_Cockpit_",
            CategoryId = "Cockpit",
            AutoAddCategoryIdIfMissing = true
        };
        public ValueChange<float> AutoFixCockpitTonnageChange = new ValueChange<float> {From = 0, By = 3};
        public ValueChange<int> AutoFixCockpitSlotChange = new ValueChange<int> {From = 1, By = 0};

        public IdentityHelper AutoFixLegUpgradesCategorizer = new IdentityHelper
        {
            AllowedLocations = ChassisLocations.Legs,
            ComponentType = ComponentType.Upgrade,
            Prefix = null, //"Gear_Actuator_";
            CategoryId = "LegFootActuator",
            AutoAddCategoryIdIfMissing = true
        };
        public ValueChange<int> AutoFixLegUpgradesSlotChange = new ValueChange<int>  {From = 3, By = -1, FromIsMin = true, NewMin = 1};

        public string[] AutoFixChassisDefSkip = { };

        public ChassisSlotsChange[] AutoFixChassisDefSlotsChanges =
        {
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
        public class ChassisSlotsChange
        {
            public ChassisLocations Location;
            public ValueChange<int> Change;
        }

        public bool AutoFixChassisDefInitialTonnage = true;
        public float AutoFixChassisDefInitialToTotalTonnageFactor = 0.1f; // 10% structure weight
        public bool AutoFixChassisDefMaxJumpjets = true;
        public int AutoFixChassisDefMaxJumpjetsCount = 8;
        public int AutoFixChassisDefMaxJumpjetsRating = 400;

        public WeaponSlotChange[] AutoFixWeaponDefSlotsChanges =
        {
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
            }
        };
        public class WeaponSlotChange
        {
            public WeaponSubType Type;
            public ValueChange<int> Change;
        }

        #endregion

        #region arm actuators

        public bool UseArmActuators = true;
        public bool ForceFullDefaultActuators = false;
        public string IgnoreFullActuatorsTag = null;
        public string DefaultCBTShoulder = "emod_arm_part_shoulder";
        public string DefaultCBTLower = "emod_arm_part_lower";
        public string DefaultCBTUpper = "emod_arm_part_upper";
        public string DefaultCBTHand = "emod_arm_part_hand";
        public string DefaultCBTDefLower = "emod_arm_part_lower";
        public string DefaultCBTDefHand = "emod_arm_part_hand";
        public bool InterruptHandDropIfNoLower = false;
        public bool ExtendHandLimit = true;

        #endregion
        
        #region movement

        public CBTMovementSettings CBTMovement = new CBTMovementSettings();
        public class CBTMovementSettings
        {
            /* 
            set to false to use TT walk values
            using the default game values, slow mechs move a bit faster, and fast mechs move a bit slower
            Examples if set to true:
                Walk 2  70 / 125
                Walk 3  95 / 165
                Walk 4 120 / 200
                Walk 5 140 / 240
                Walk 6 165 / 275
                Walk 7 190 / 315
                Walk 8 210 / 350
            */
            public bool UseGameWalkValues = true;

            //// set to false to only allow engines that produce integer walk values
            //public bool AllowNonIntWalkValues = true;

            // this setting controls if the allowed number of jump jets is rounded up or down
            // example: if false, TT walk speed of 2.1 allows 2 jump jets, if true, it allows 3 jump jets
            public bool JJRoundUp = false;

            /*
            not sure why you would want to change these, but they are set here
            they are the multiples that translate TT movement values to game movement values
            Example:
                A griffin that walks 5 would walk 5 * 30 = 150 and sprint 5 * 50 = 250
            NOTE: if you have the UseGameWalkValues set, the exact values are then changed based on a linear equasion
            */
            public float TTWalkMultiplier = 30f;
            public float TTSprintMultiplier = 50f;
        }

        #endregion

        #region hardpoint fixes

        public HardpointFixSettings HardpointFix = new HardpointFixSettings();
        public class HardpointFixSettings
        {
            // TODO add set to 4 slots per chassis location autofix variant
            // TODO make enum so we have: set to 4, set to encountered prefabs, disabled
            public bool AutoFixChassisDefWeaponHardpointCounts = false; // true = hardpoint counts derived from prefab hardpoints
            public bool EnforceHardpointLimits = false; // true = use prefab hardpoints
            public bool AllowDefaultLoadoutWeapons = false;
            
            // from: /data/weapon$ grep -R "PrefabIdentifier" . | cut -d\" -f 4 | sort | uniq
            // to: /data/hardpoints$ grep -R "chrPrfWeap" . | cut -d_ -f 5 | sort | uniq
            // default mapping = prefabid -> lower case prefab id (e.g. Flamer -> flamer, PPC -> ppc)
            public WeaponPrefabMapping[] WeaponPrefabMappings = new WeaponPrefabMapping[]
            {
                new WeaponPrefabMapping
                {
                    PrefabIdentifier= "AC2",
                    HardpointCandidates = new[] {"ac2", "ac", "lbx"}
                },
                new WeaponPrefabMapping
                {
                    PrefabIdentifier= "AC5",
                    HardpointCandidates = new[] {"ac5", "uac5", "ac", "lbx"}
                },
                new WeaponPrefabMapping
                {
                    PrefabIdentifier= "AC10",
                    HardpointCandidates = new[] {"ac10", "lbx10", "ac", "lbx"}
                },
                new WeaponPrefabMapping
                {
                    PrefabIdentifier= "AC20",
                    HardpointCandidates = new[] {"ac20", "ac", "lbx"}
                },
                new WeaponPrefabMapping
                { /* requested by LtShade */
                    PrefabIdentifier= "artillery",
                    HardpointCandidates = new[] {"artillery", "ac20", "ac", "lbx"}
                },
                new WeaponPrefabMapping
                {
                    PrefabIdentifier= "lrm5",
                    HardpointCandidates = new[] {"lrm5", "lrm10", "lrm15", "lrm20", "srm20"}
                },
                new WeaponPrefabMapping
                {
                    PrefabIdentifier= "lrm10",
                    HardpointCandidates = new[] {"lrm10", "lrm15", "lrm20", "srm20", "lrm5"}
                },
                new WeaponPrefabMapping
                {
                    PrefabIdentifier= "lrm15",
                    HardpointCandidates = new[] {"lrm15", "lrm20", "srm20", "lrm10", "lrm5"}
                },
                new WeaponPrefabMapping
                {
                    PrefabIdentifier= "lrm20",
                    HardpointCandidates = new[] {"lrm20", "srm20", "lrm15", "lrm10", "lrm5"}
                },
                new WeaponPrefabMapping
                {
                    PrefabIdentifier= "MachineGun",
                    HardpointCandidates = new[] {"machinegun", "mg"}
                },
                new WeaponPrefabMapping
                {
                    PrefabIdentifier= "srm2",
                    HardpointCandidates = new[] {"srm2", "srm4", "srm6"}
                },
                new WeaponPrefabMapping
                {
                    PrefabIdentifier= "srm4",
                    HardpointCandidates = new[] {"srm4", "srm6", "srm2"}
                },
                new WeaponPrefabMapping
                {
                    PrefabIdentifier= "srm6",
                    HardpointCandidates = new[] {"srm6", "srm4", "srm2"}
                }
            };

            public class WeaponPrefabMapping
            {
                public string PrefabIdentifier;
                public string[] HardpointCandidates;
            }
        }

        #endregion
    }
}
