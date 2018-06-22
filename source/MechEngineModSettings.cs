using DynModLib;

namespace MechEngineMod
{
    public class MechEngineModSettings : ModSettings
    {
        public int TechCostPerEngineTon = 1;
        public int EngineMissingFallbackHeatSinkCapacity = 30; // for stuff that wasn't auto fixed and still missing an engine, use a fallback

        public bool EngineCritsEnabled = true;
        public int EngineHeatSinkCapacityAdjustmentPerCrit = -15;
        
        public string[] AutoFixMechDefSkip = { }; // mech defs to skip for AutoFixMechDef*

        public bool AutoFixMechDefEngine = true; // adds missing engine and removes too many jump jets
        public string AutoFixEnginePrefix = "emod_engine_std"; // use these types of engines for the autofix

        public bool AutoFixMechDefGyro = true; // adds missing gyro
        public string AutoFixMechDefGyroId = "Gear_Gyro_Generic_Standard";
        public bool AutoFixGyroUpgrades = true; // enlarges gyro upgrades that are size 3 to size 4

        public bool AutoFixMechDefCockpit = true; // adds missing cockpit
        public string AutoFixMechDefCockpitId = "Gear_Cockpit_Generic_Standard";
        public bool AutoFixCockpitUpgrades = true; // adds 3 tons to cockpit upgrades that weigh 0 tons
        
        public string[] AutoFixChassisDefSkip = { };
        public bool AutoFixChassisDefSlots = true; // adds 2 torso slots at a cost of 2 leg slots per side if they match stock slot layouts
        public bool AutoFixChassisDefInitialTonnage = true;
        public float AutoFixInitialToTotalTonnageFactor = 0.1f; // 10% structure weight
        public float AutoFixInitialFixedAddedTonnage = 0; // not used anymore, was for cockpit 3 ton before cockpit became own item

        public bool EnableAvailabilityChecks = true; // set this to false to have a faster mechlab experience on large engine counts 

        public string GearGryoPrefix = "Gear_Gyro_";
        public string GearCockpitPrefix = "Gear_Cockpit_";

        public string GearHeatSinkDouble = "Gear_HeatSink_Generic_Double";
        public string GearHeatSinkStandard = "Gear_HeatSink_Generic_Standard";
        public string EngineKitDHS = "emod_kit_dhs";

        public string EnginePartPrefix = "emod_engine";
        public string MainEnginePrefix = "emod_engine_";
        public EngineType[] EngineTypes = {
            new EngineType { Prefix = "emod_engine_std", Requirements = new string[] {} },
            new EngineType { Prefix = "emod_engine_compact", Requirements = new string[] {} },
            new EngineType { Prefix = "emod_engine_xl", Requirements = new[] {"emod_engineslots_xl_left", "emod_engineslots_xl_right"} },
            new EngineType { Prefix = "emod_engine_xxl", Requirements = new[] {"emod_engineslots_xxl_left", "emod_engineslots_xxl_right"} },
            new EngineType { Prefix = "emod_engine_cxl", Requirements = new[] {"emod_engineslots_cxl_left", "emod_engineslots_cxl_right"} },
            new EngineType { Prefix = "emod_engine_cxxl", Requirements = new[] {"emod_engineslots_cxxl_left", "emod_engineslots_cxxl_right"} },
            new EngineType { Prefix = "emod_engine_light", Requirements = new[] {"emod_engineslots_light_left", "emod_engineslots_light_right"} }
        };
        
        public bool AllowMixingDoubleAndSingleHeatSinks = false; // only useful for patchwork like behavior
        public bool FractionalAccounting = false; // not applied for engines and might not match 100% due to based on non-fractional accounting numbers
        public bool AllowPartialWeightSavings = false; // similar to patchwork armor without any penalties and location requirements, also works for structure

        public string StructurePrefix = "emod_structureslots_";
        public WeightSavingSlotType[] StructureTypes = {
            new WeightSavingSlotType { ComponentDefId = "emod_structureslots_endosteel", RequiredCriticalSlotCount = 14, WeightSavingsFactor = 0.5f },
            new WeightSavingSlotType { ComponentDefId = "emod_structureslots_endocomposite", RequiredCriticalSlotCount = 7, WeightSavingsFactor = 0.25f },
            new WeightSavingSlotType { ComponentDefId = "emod_structureslots_clanendosteel", RequiredCriticalSlotCount = 7, WeightSavingsFactor = 0.5f },
        };

        public string ArmorPrefix = "emod_armorslots_";
        public WeightSavingSlotType[] ArmorTypes = {
            new WeightSavingSlotType { ComponentDefId = "emod_armorslots_lightferrosfibrous", RequiredCriticalSlotCount = 7, WeightSavingsFactor = 1f - 1f / 1.06f },
            new WeightSavingSlotType { ComponentDefId = "emod_armorslots_ferrosfibrous", RequiredCriticalSlotCount = 14, WeightSavingsFactor = 1f - 1f / 1.12f },
            new WeightSavingSlotType { ComponentDefId = "emod_armorslots_clanferrosfibrous", RequiredCriticalSlotCount = 7, WeightSavingsFactor = 1f - 1f / 1.2f },
            new WeightSavingSlotType { ComponentDefId = "emod_armorslots_stealth", RequiredCriticalSlotCount = 6, WeightSavingsFactor = 1f },
            new WeightSavingSlotType { ComponentDefId = "emod_armorslots_heavyferrosfibrous", RequiredCriticalSlotCount = 21, WeightSavingsFactor = 1f - 1f / 1.24f },
        };

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
        public float const_TTWalkMultiplier = 30f;
        public float const_TTSprintMultiplier = 50f;
    }

    public class EngineType
    {
        public string Prefix;
        public string[] Requirements;
    }

    public class WeightSavingSlotType
    {
        public string ComponentDefId;
        public int RequiredCriticalSlotCount;
        public float WeightSavingsFactor;
    }
}
