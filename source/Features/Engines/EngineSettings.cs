namespace MechEngineer.Features.Engines
{
    internal class EngineSettings : BaseSettings
    {
        public int MinimumHeatSinksOnMech = 10; // minimum heatsinks a mech requires
        public bool EnforceRulesForAdditionalInternalHeatSinks = true; // can't have those juicy ++ cooling systems with smaller fusion cores than the rules allow it
        public bool AllowMixingHeatSinkTypes = false; // only useful for patchwork like behavior
        public string DefaultEngineHeatSinkId = "Gear_HeatSink_Generic_Standard"; // default heat sink type for engines without a kit
        public int EngineMissingFallbackHeatSinkCapacity = 30; // for stuff that wasn't auto fixed and still missing an engine, use a fallback

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
}