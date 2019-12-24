using BattleTech;

namespace MechEngineer.Features.ArmorStructureChanges
{
    internal static class StatCollectionExtension
    {
        internal static StatisticHelper<float> ArmorMultiplier(this StatCollection statCollection)
        {
            return new StatisticHelper<float>("ArmorMultiplier", statCollection);
        }

        internal static StatisticHelper<float> StructureMultiplier(this StatCollection statCollection)
        {
            return new StatisticHelper<float>("StructureMultiplier", statCollection);
        }
    }
}