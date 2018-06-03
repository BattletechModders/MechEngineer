using DynModLib;

namespace MechEngineMod
{
    public class MechEngineModSettings : ModSettings
    {
        public int TechCostPerEngineTon = 1;
        public int FallbackHeatSinkCount = 10; // for stuff that wasn't auto fixed

        public float SpeedMultiplierPerDamagedEnginePart = 1.0f; // no speed reduction
        public int HeatSinkCapacityPerDamagedEnginePart = -15;

        public bool AutoFixMechDefs = true; // adds missing engine and removes too many jump jets
        public string[] AutoFixMechDefsSkip = {};
        public bool AutoFixChassisDefs = true;
        public string[] AutoFixChassisDefsSkip = {};
        public float AutoFixInitialToTotalTonnageFactor = 0.1f; // 10% structure weight
        public float AutoFixInitialFixedAddedTonnage = 3; // 3 for cockpit

        public bool EndoSteelRequireAllSlots = true;
        public int EndoSteelRequiredCriticals = 14;
        public float EndoSteelStructureWeightSavingsFactor = 0.5f; // based on initial tonnage

        public bool FerroFibrousRequireAllSlots = true;
        public int FerrosFibrousRequiredCriticals = 14;
        public float FerrosFibrousArmorWeightSavingsFactor = 1f / 1.12f;

        public bool AllowMixingDoubleAndSingleHeatSinks = false;

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
		
        // set to false to only allow engines that produce integer walk values
        public bool AllowNonIntWalkValues = true;
		
        // this setting controls if the allowed number of jump jets is rounded up or down
        // example: if false, TT walk speed of 2.1 allows 2 jump jets, if true, it allows 3 jump jets
        public bool JJRoundUp = false;
		
        // this controls the maximum (global) allowed jump jets.
        // currently set to 8 in case the game can't handle anymore
        public int const_MaxNumberOfJJ = 8;
		
        /*
		these numbers determine how fast and slow mechs are allowed to go
		They are given in tabletop walk numbers.
		Examples: 
			urbanmech walk = 2
			griffin walk = 5
			spider walk = 8
		*/
        public int const_MinTTWalk = 2;
        public int const_MaxTTWalk = 8;
		
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
}