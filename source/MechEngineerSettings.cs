using System.Collections.Generic;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    public class AddHelper
    {
        public string ComponentDefId { get; set; }
        public ChassisLocations ChassisLocation { get; set; }
        public ComponentType ComponentType { get; set; }
    }

    public class MechEngineerSettings : ModSettings
    {
        #region misc

        public bool AllowMixingHeatSinkTypes = false; // only useful for patchwork like behavior
        public bool FractionalAccounting = false; // instead of half ton rounding use kg precise calculations
        //public bool AllowPartialWeightSavings = false; // similar to patchwork armor without any penalties and location requirements, also works for structure

        public bool SaveMechDefOnMechLabConfirm = false;

        public bool PerformanceEnableAvailabilityChecks = true; // set this to false to have a faster mechlab experience on large engine counts (300+ item types)

        public string DefaultEngineHeatSinkId = "Gear_HeatSink_Generic_Standard"; // default heat sink type for engines without a kit

        public int EngineMissingFallbackHeatSinkCapacity = 30; // for stuff that wasn't auto fixed and still missing an engine, use a fallback

        public CriticalHitStates EngineCriticalHitStates = new CriticalHitStates
        {
            MaxStates = 2,
            DeathMethod = DeathMethod.EngineDestroyed,
            HitEffects = new[]
            {
                new CriticalHitEffect
                {
                    State = 1,
                    StatusEffect = new EffectData
                    {
                        durationData = new EffectDurationData(),
                        targetingData = new EffectTargetingData
                        {
                            effectTriggerType = EffectTriggerType.Passive,
                            effectTargetType = EffectTargetType.Creator,
                            showInTargetPreview = true,
                            showInStatusPanel = true
                        },
                        effectType = EffectType.StatisticEffect,
                        Description = new BaseDescriptionDef(
                            Id:"StatusEffect-EngineCrit1",
                            Name:"Engine Crit (1)",
                            Details:"Engine was hit, additional 15 Heat / Turn",
                            Icon:"uixSvgIcon_equipment_ThermalExchanger"
                            ),
                        nature = EffectNature.Debuff,
                        statisticData = new StatisticEffectData
                        {
                            statName = "HeatSinkCapacity",
                            operation = StatCollection.StatOperation.Int_Subtract,
                            modValue = "15",
                            modType = "System.Int32"
                        }
                    }
                },
                new CriticalHitEffect
                {
                    State = 2,
                    StatusEffect = new EffectData
                    {
                        durationData = new EffectDurationData(),
                        targetingData = new EffectTargetingData
                        {
                            effectTriggerType = EffectTriggerType.Passive,
                            effectTargetType = EffectTargetType.Creator,
                            showInTargetPreview = true,
                            showInStatusPanel = true
                        },
                        effectType = EffectType.StatisticEffect,
                        Description = new BaseDescriptionDef(
                            Id:"StatusEffect-EngineCrit2",
                            Name:"Engine Crit (2)",
                            Details:"Engine was hit, additional 30 Heat / Turn",
                            Icon:"uixSvgIcon_equipment_ThermalExchanger"
                        ),
                        nature = EffectNature.Debuff,
                        statisticData = new StatisticEffectData
                        {
                            statName = "HeatSinkCapacity",
                            operation = StatCollection.StatOperation.Int_Subtract,
                            modValue = "30",
                            modType = "System.Int32"
                        }
                    }
                }
            }
        };

        // MWO does not allow to drop if that would mean to go overweight
        // battletech allows overweight, to stay consistent so we also allow overspace usage by default
        // set to true to switch to MWO style
        public bool MWOStyleDontAlowDropIfNotEnoughSpaceForDynamics = false;

        #endregion

        #region auto fixes

        public string[] AutoFixMechDefSkip = { }; // mech defs to skip for AutoFixMechDef*

        public bool AutoFixMechDefEngine = true; // adds missing engine and removes too many jump jets
        public string AutoFixMechDefEngineTypeDef = "emod_engineslots_std_center"; // always assumes weight factor 1.0

        public IdentityHelper AutoFixGyroCategorizer = new IdentityHelper
        {
            AllowedLocations = ChassisLocations.CenterTorso, // optional if category is properly setup
            ComponentType = ComponentType.Upgrade, // optional if category is properly setup
            Prefix = "Gear_Gyro_", // optional if category is properly setup
            CategoryId = "Gyro", // required
            AutoAddCategoryIdIfMissing = true // adds category id to items matched by optional filters
        };
        public AddHelper AutoFixMechDefGyroAdder = new AddHelper
        {
            ChassisLocation = ChassisLocations.CenterTorso,
            ComponentDefId = "Gear_Gyro_Generic_Standard",
            ComponentType = ComponentType.Upgrade,
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
        public AddHelper AutoFixMechDefCockpitAdder = new AddHelper
        {
            ChassisLocation = ChassisLocations.Head,
            ComponentDefId = "Gear_Cockpit_Generic_Standard",
            ComponentType = ComponentType.Upgrade,
        };
        public ValueChange<float> AutoFixCockpitTonnageChange = new ValueChange<float> {From = 0, By = 3};
        public ValueChange<int> AutoFixCockpitSlotChange = null; //new ValueChange<int> {From = 1, By = 0};

        public IdentityHelper AutoFixLegUpgradesCategorizer = new IdentityHelper
        {
            AllowedLocations = ChassisLocations.Legs,
            ComponentType = ComponentType.Upgrade,
            Prefix = null, //"Gear_Actuator_";
            CategoryId = "LegUpgrade",
            AutoAddCategoryIdIfMissing = true
        };
        public ValueChange<int> AutoFixLegUpgradesSlotChange = new ValueChange<int> {From = 3, By = -2, FromIsMin = true, NewMin = 1};

        public IdentityHelper AutoFixArmorCategorizer = new IdentityHelper
        {
            CategoryId = "Armor",
            AutoAddCategoryIdIfMissing = true
        };
        public AddHelper AutoFixMechDefArmorAdder = new AddHelper
        {
            ChassisLocation = ChassisLocations.CenterTorso,
            ComponentDefId = "emod_armorslots_standard",
            ComponentType = ComponentType.Upgrade,
        };

        public IdentityHelper AutoFixStructureCategorizer = new IdentityHelper
        {
            CategoryId = "Structure",
            AutoAddCategoryIdIfMissing = true
        };

        public AddHelper AutoFixMechDefStructureAdder = new AddHelper
        {
            ChassisLocation = ChassisLocations.CenterTorso,
            ComponentDefId = "emod_structureslots_standard",
            ComponentType = ComponentType.Upgrade,
        };

        public string[] AutoFixChassisDefSkip = { };
        public bool AutoFixChassisDefSlots = true; // adds 2 torso slots at a cost of 2 leg slots per side if they match stock slot layouts
        public Dictionary<string, ValueChange<int>> AutoFixChassisDefSlotsChanges = new Dictionary<string, ValueChange<int>>
        {
            [ChassisLocations.LeftTorso.ToString()] = new ValueChange<int> {From = 10, By = 2},
            [ChassisLocations.RightTorso.ToString()] = new ValueChange<int> {From = 10, By = 2},
            [ChassisLocations.LeftLeg.ToString()] = new ValueChange<int> {From = 4, By = -2},
            [ChassisLocations.RightLeg.ToString()] = new ValueChange<int> {From = 4, By = -2},
            [ChassisLocations.Head.ToString()] = new ValueChange<int> {From = 1, By = 1}, // 2
            [ChassisLocations.CenterTorso.ToString()] = new ValueChange<int> {From = 4, By = 10},
        };

        public bool AutoFixChassisDefInitialTonnage = true;
        public float AutoFixChassisDefInitialToTotalTonnageFactor = 0.1f; // 10% structure weight

        public bool AutoFixWeaponDefSlots = true;
        public Dictionary<string, ValueChange<int>> AutoFixWeaponDefSlotsChanges = new Dictionary<string, ValueChange<int>>
        {
            [WeaponSubType.AC5.ToString()] = new ValueChange<int> {From = 2, By = 2},
            [WeaponSubType.AC10.ToString()] = new ValueChange<int> {From = 3, By = 4},
            [WeaponSubType.AC20.ToString()] = new ValueChange<int> {From = 4, By = 4}, //6
            [WeaponSubType.Gauss.ToString()] = new ValueChange<int> {From = 5, By = 2},
            [WeaponSubType.LargeLaserPulse.ToString()] = new ValueChange<int> {From = 2, By = 1},
            [WeaponSubType.LRM20.ToString()] = new ValueChange<int> {From = 4, By = 1},
        };

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
            public bool AllowLRMInSmallerSlotsForAll = false;
            public string[] AllowLRMInSmallerSlotsForMechs = { "atlas" };
            public bool AllowLRMInLargerSlotsForAll = true;
        }

        #endregion

        #region categories

        public CategoryDescriptor[] Categories = {
            new CategoryDescriptor
            {
                Name = "Armor",
                displayName = "Armor",
                MaxEquiped =  1,
                MinEquiped =  1,
                AutoReplace = true,
                DefaultCustoms = new Dictionary<string, object>
                {
                    ["Sorter"] = new Dictionary<string, object>{ ["Order"] = 0 },
                    ["Replace"] = new Dictionary<string, object>{ ["ComponentDefId"] = "emod_armorslots_standard" }
                }
            },
            new CategoryDescriptor
            {
                Name = "Structure",
                displayName = "Structure",
                MaxEquiped =  1,
                MinEquiped =  1,
                AutoReplace = true,
                DefaultCustoms = new Dictionary<string, object>
                {
                    ["Sorter"] = new Dictionary<string, object>{ ["Order"] = 1 },
                    ["Replace"] = new Dictionary<string, object>{ ["ComponentDefId"] = "emod_structureslots_standard" }
                }
            },
            new CategoryDescriptor
            {
                Name = "Cockpit",
                displayName = "Cockpit",
                MaxEquiped =  1,
                MinEquiped =  1,
                AutoReplace = true,
                DefaultCustoms = new Dictionary<string, object>
                {
                    ["Sorter"] = new Dictionary<string, object>{ ["Order"] = 0 },
                    ["Replace"] = new Dictionary<string, object>{ ["ComponentDefId"] = "Gear_Cockpit_Generic_Standard" }
                }
            },
            new CategoryDescriptor
            {
                Name = "Gyro",
                displayName = "Gyro",
                MaxEquiped =  1,
                MinEquiped =  1,
                AutoReplace = true,
                DefaultCustoms = new Dictionary<string, object>
                {
                    ["Sorter"] = new Dictionary<string, object>{ ["Order"] = 2 },
                    ["Replace"] = new Dictionary<string, object>{ ["ComponentDefId"] = "Gear_Gyro_Generic_Standard" }
                }
            },
            new CategoryDescriptor
            {
                Name = "EngineShield",
                displayName = "Engine Shielding",
                MaxEquiped =  1,
                MinEquiped =  1,
                AutoReplace = true,
                DefaultCustoms = new Dictionary<string, object>
                {
                    ["Sorter"] = new Dictionary<string, object>{ ["Order"] = 3 },
                    ["Replace"] = new Dictionary<string, object>{ ["ComponentDefId"] = "emod_engineslots_std_center" }
                }
            },
            new CategoryDescriptor
            {
                Name = "EngineCore",
                displayName = "Engine Core",
                MaxEquiped =  1,
                MinEquiped =  1,
                AutoReplace = true,
                DefaultCustoms = new Dictionary<string, object>
                {
                    ["Sorter"] = new Dictionary<string, object>{ ["Order"] = 4 },
                    ["Replace"] = new Dictionary<string, object>{ ["ComponentDefId"] = "emod_engine_dummy" }
                }
            },
            new CategoryDescriptor
            {
                Name = "LegUpgrade",
                displayName = "Leg Upgrade",
                MaxEquiped =  1,
                AutoReplace = true
            },


        };

        #endregion
    }
}
