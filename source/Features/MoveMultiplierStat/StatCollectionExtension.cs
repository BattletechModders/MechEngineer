using BattleTech;

namespace MechEngineer.Features.MoveMultiplierStat
{
    internal static class StatCollectionExtension
    {
        internal static StatisticAdapter<float> MoveMultiplier(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("MoveMultiplier", statCollection, 1f);
        }
    }
}