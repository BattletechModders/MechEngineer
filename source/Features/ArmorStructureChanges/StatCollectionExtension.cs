using BattleTech;
using MechEngineer.Helper;

namespace MechEngineer.Features.ArmorStructureChanges;

internal static class StatCollectionExtension
{
    internal static StatisticAdapter<float> ArmorMultiplier(this StatCollection statCollection)
    {
        return new("ArmorMultiplier", statCollection, 1);
    }

    internal static StatisticAdapter<float> StructureMultiplier(this StatCollection statCollection)
    {
        return new("StructureMultiplier", statCollection, 1);
    }
}