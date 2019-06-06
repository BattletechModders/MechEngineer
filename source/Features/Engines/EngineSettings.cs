namespace MechEngineer.Features.Engines
{
    internal class EngineSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Provides engines that can be installed on mechs similar to how CBT works.";
        
        public bool AllowMountingAllRatings = false;
        public string AllowMountingAllRatingsDescription = "Allow mounting all fusion core rating regardless of min/max sprint factors.";

        public int MinimumHeatSinksOnMech = 10;
        public string MinimumHeatSinksOnMechDescription = "Minimum heatsinks a mech requires.";

        public bool EnforceRulesForAdditionalInternalHeatSinks = true;
        public string EnforceRulesForAdditionalInternalHeatSinksDescription = "Can't have those juicy ++ cooling systems with smaller fusion cores than the rules allow it.";

        public bool AllowMixingHeatSinkTypes = false;
        public string AllowMixingHeatSinkTypesDescription = "Allow heat sinks patchwork.";

        public string DefaultEngineHeatSinkId = "Gear_HeatSink_Generic_Standard";
        public string DefaultEngineHeatSinkIdDescription = "Default heat sink type for engines without a kit.";

        public int EngineMissingFallbackHeatSinkCapacity = 30;
        public string EngineMissingFallbackHeatSinkCapacityDescription = "Heat sink capacity if no engine is detected.";

        /* 
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
        public string UseGameWalkValuesDescription = "Set to false to use more TT like walk values.";

        // example: if false, TT walk speed of 2.1 allows 2 jump jets, if true, it allows 3 jump jets
        public bool JJRoundUp = false;
        public string JJRoundUpDescription = "The allowed number of jump jets is rounded up or down.";

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
}