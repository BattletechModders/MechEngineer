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

        public float MovementPointDistanceMultiplier = 24f;
        public string MovementPointDistanceMultiplierDescription => "The distance of a TT movement point, 24 is vanilla CombatGameConstants.ExperimentalGridDistance .";

        public float MinimumJumpDistanceForHeat = 3 * 24f;
        public string MinimumJumpDistanceForHeatDescription => "The minimum distance to use for calculating jump heat, any jumps shorter will still produce the equivalent heat. CBT Rule: 3 MP.";

        public float RunMultiplier = 1.5f;
        public string RunMultiplierDescription => "How much faster running is than walking.";

        public bool AutoConvertJumpCapacityInDefToStat = true;
        public string AutoConvertJumpCapacityInDefToStatDescription => "All jump jets that have a JumpCapacity definied in the Def will have it auto-converted to the JumpCapacity stat.";

        public bool LimitEngineCoresToTonnage = true;
        public string IgnoreLimitEngineChassisTag = "";
    }
}