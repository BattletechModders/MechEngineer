
namespace MechEngineer
{
    public class MechEngineerSettings : ModSettings
    {
        public bool TestEnableAllTags = true;

        public int EngineMissingFallbackHeatSinkCapacity = 30; // for stuff that wasn't auto fixed and still missing an engine, use a fallback

        public bool EngineCritsEnabled = true;
        public int EngineHeatSinkCapacityAdjustmentPerCrit = -15;
        
        public string[] AutoFixMechDefSkip = { }; // mech defs to skip for AutoFixMechDef*

        public bool AutoFixMechDefEngine = true; // adds missing engine and removes too many jump jets
        public string AutoFixMechDefEngineTypeDef = "emod_engineslots_std_center"; // always assumes weight factor 1.0

        public bool AutoFixMechDefGyro = true; // adds missing gyro
        public string AutoFixMechDefGyroId = "Gear_Gyro_Generic_Standard";
        public bool AutoFixGyroUpgrades = true; // enlarges gyro upgrades that are size 3 to size 4
        public string AutoFixGyroPrefix = "Gear_Gyro_"; // "Gear_Gyro_";

        public bool AutoFixMechDefCockpit = true; // adds missing cockpit
        public string AutoFixMechDefCockpitId = "Gear_Cockpit_Generic_Standard";
        public bool AutoFixCockpitUpgrades = true; // adds 3 tons to cockpit upgrades that weigh 0 tons
        public string AutoFixCockpitPrefix = "Gear_Cockpit_"; // "Gear_Cockpit_";

        public bool AutoFixLegUpgrades = true; // reduces leg upgrades from 3 to 1 size
        public string AutoFixLegUpgradesPrefix = null; //"Gear_Actuator_";
        
        public string[] AutoFixChassisDefSkip = { };
        public bool AutoFixChassisDefSlots = true; // adds 2 torso slots at a cost of 2 leg slots per side if they match stock slot layouts
        public bool AutoFixChassisDefInitialTonnage = true;
        public float AutoFixChassisDefInitialToTotalTonnageFactor = 0.1f; // 10% structure weight

        public bool EnableAvailabilityChecks = true; // set this to false to have a faster mechlab experience on large engine counts (300+ item types)

        public string DefaultEngineHeatSinkId = "Gear_HeatSink_Generic_Standard"; // default heat sink type for engines
        
        public bool AllowMixingHeatSinkTypes = false; // only useful for patchwork like behavior
        public bool FractionalAccounting = false; // instead of half ton rounding use kg precise calculations
        //public bool AllowPartialWeightSavings = false; // similar to patchwork armor without any penalties and location requirements, also works for structure

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

        #region HardpointFix
        
        public bool AutoFixChassisDefWeaponHardpointCounts = false; // true = hardpoint counts derived from prefab hardpoints
        public bool EnforceHardpointLimits = false; // true = use prefab hardpoints
        public bool AllowDefaultLoadoutWeapons = false;
        public bool AllowLRMInSmallerSlotsForAll = false;
        public string[] AllowLRMInSmallerSlotsForMechs = { "atlas" };
        public bool AllowLRMInLargerSlotsForAll = true;

        #endregion
    }
}
