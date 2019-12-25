using BattleTech;

namespace MechEngineer.Features.ArmorStructureChanges
{
    internal static class StatCollectionExtension
    {
        internal static StatisticAdapter<float> ArmorMultiplier(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("ArmorMultiplier", statCollection, 1);
        }

        internal static StatisticAdapter<float> StructureMultiplier(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("StructureMultiplier", statCollection, 1);
        }
    }
}