using BattleTech;

namespace MechEngineer.Features.Engines.Helper
{
    internal static class StatCollectionExtension
    {
        internal static StatisticHelper<float> JumpCapacity(this StatCollection statCollection)
        {
            return new StatisticHelper<float>("JumpCapacity", statCollection);
        }

        internal static StatisticHelper<float> JumpHeat(this StatCollection statCollection)
        {
            return new StatisticHelper<float>("JumpHeat", statCollection);
        }

        internal static StatisticHelper<float> JumpDistanceMultiplier(this StatCollection statCollection)
        {
            return new StatisticHelper<float>("JumpDistanceMultiplier", statCollection);
        }

        internal static StatisticHelper<float> WalkSpeed(this StatCollection statCollection)
        {
            return new StatisticHelper<float>("WalkSpeed", statCollection);
        }

        internal static StatisticHelper<float> RunSpeed(this StatCollection statCollection)
        {
            return new StatisticHelper<float>("RunSpeed", statCollection);
        }
    }
}