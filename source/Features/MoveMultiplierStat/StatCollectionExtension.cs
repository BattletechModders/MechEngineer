using BattleTech;
using MechEngineer.Helper;

namespace MechEngineer.Features.MoveMultiplierStat;

internal static class StatCollectionExtension
{
    internal static StatisticAdapter<float> MoveMultiplier(this StatCollection statCollection)
    {
        return new("MoveMultiplier", statCollection, 1f);
    }
}