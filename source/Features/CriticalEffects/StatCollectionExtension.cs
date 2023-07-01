using BattleTech;
using MechEngineer.Helper;

namespace MechEngineer.Features.CriticalEffects;

internal static class StatCollectionExtension
{
    internal static StatisticAdapter<int> MECriticalSlotsHit(this StatCollection statCollection)
    {
        return new("MECriticalSlotsHit", statCollection, 0);
    }

    internal static StatisticAdapter<int> MECriticalSlotsHitArmored(this StatCollection statCollection)
    {
        return new("MECriticalSlotsHitArmored", statCollection, 0);
    }
}