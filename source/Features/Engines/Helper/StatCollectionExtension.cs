using BattleTech;

namespace MechEngineer.Features.Engines.Helper
{
    internal static class StatCollectionExtension
    {
        internal static StatisticAdapter<float> JumpCapacity(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("JumpCapacity", statCollection, 0);
        }

        internal static StatisticAdapter<float> JumpHeat(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("JumpHeat", statCollection, 0);
        }

        internal static StatisticAdapter<float> JumpDistanceMultiplier(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("JumpDistanceMultiplier", statCollection, 1);
        }

        internal static StatisticAdapter<float> WalkSpeed(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("WalkSpeed", statCollection, 0);
        }

        internal static StatisticAdapter<float> RunSpeed(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("RunSpeed", statCollection, 0);
        }
    }
}