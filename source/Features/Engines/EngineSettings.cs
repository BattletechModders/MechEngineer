using BattleTech;
using MechEngineer.Features.Engines.Helper;

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

        public bool CBTWalkAndRunMPRounding = false;
        public string CBTWalkAndRunMPRoundingDescription => "If true, walking MPs are rounded down and running MPs are rounded up.";

        public float MovementPointDistanceMultiplier = 24f;
        public string MovementPointDistanceMultiplierDescription => "The distance of a TT movement point, 24 is vanilla CombatGameConstants.ExperimentalGridDistance .";

        public float? JumpJetMovementPointDistanceMultiplier = null;
        public string JumpJetMovementPointDistanceMultiplierDescription => $"The distance of a TT movement point when calculating jump distances, if undefined falls back to {nameof(MovementPointDistanceMultiplier)}.";

        public float MinimJumpHeat = 3f * 3f;
        public string MinimJumpHeatDescription => "Minimum heat when doing a jump, even if only one jump jet exists and only when jumping one hex.";

        public bool AutoConvertJumpCapacityInDefToStat = true;
        public string AutoConvertJumpCapacityInDefToStatDescription => $"All {nameof(JumpJetDef.JumpCapacity)} values in JumpJetDefs will be auto-converted to the {nameof(StatCollectionExtension.JumpCapacity)} statistic.";

        public float? JumpJetDefaultJumpHeat = 3;
        public string JumpJetDefaultJumpHeatDescription => $"The heat the jump jet produces when fully* used (* jumping below max distance reduces produced heat). Can be adjusted using the {nameof(StatCollectionExtension.JumpHeat)} statistic.";

        public float RunMultiplier = 1.5f;
        public string RunMultiplierDescription => "How much faster running is than walking.";

        public int EngineRatingForChassisMovementStat = 250;
        public string EngineRatingForChassisMovementStatDescription => "The engine rating to use when evaluating the movement stat of a mech.";

        public bool LimitEngineCoresToTonnage = true;
        public string IgnoreLimitEngineChassisTag = "";
    }
}