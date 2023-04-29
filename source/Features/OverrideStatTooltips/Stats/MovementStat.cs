using BattleTech;
using Localize;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.OverrideStatTooltips.Helper;
using MechEngineer.Features.OverrideTonnage;

namespace MechEngineer.Features.OverrideStatTooltips.Stats;

internal class MovementStat : IStatHandler
{
    public void SetupTooltip(StatTooltipData tooltipData, MechDef mechDef)
    {
        var stats = new MechDefMovementStatistics(mechDef);

        static string DistanceToSummary(float meter)
        {
            var meters = PrecisionUtils.RoundDownToInt(meter);
            var hexWidth = MechStatisticsRules.Combat.MoveConstants.ExperimentalGridDistance;
            var hexes = PrecisionUtils.RoundDownToInt(meters / hexWidth);
            var translatedValue = Strings.T("{0}m / {1} hex", meters, hexes);
            return translatedValue;
        }

        tooltipData.dataList.Add(Strings.T("Max Move"), DistanceToSummary(stats.WalkSpeed));
        tooltipData.dataList.Add("<u>" + Strings.T("Max Sprint") + "</u>", DistanceToSummary(stats.RunSpeed));
        tooltipData.dataList.Add(Strings.T("Max Jump"), DistanceToSummary(stats.JumpDistance));
        tooltipData.dataList.Add(Strings.T("TT Walk MP"), $"{stats.WalkMovementPoint}");
    }

    public float BarValue(MechDef mechDef)
    {
        var stats = new MechDefMovementStatistics(mechDef);
        return stats.GetStatisticRating();
    }

    // TODO unused as chassis based stats are disabled for now
    internal static float BarValue(ChassisDef chassisDef)
    {
        var movement = new EngineMovement(EngineFeature.settings.EngineRatingForChassisMovementStat, chassisDef.Tonnage);
        return MechDefMovementStatistics.GetStatisticRating(movement.RunSpeed);
    }
}