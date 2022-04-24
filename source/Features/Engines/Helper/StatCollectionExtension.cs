using BattleTech;
using MechEngineer.Helper;

namespace MechEngineer.Features.Engines.Helper;

internal static class StatCollectionExtension
{
    internal static StatisticAdapter<float> JumpJetCountMultiplier(this StatCollection statCollection)
    {
        return new("JumpJetCountMultiplier", statCollection, 1);
    }

    internal static StatisticAdapter<float> JumpCapacity(this StatCollection statCollection)
    {
        return new("JumpCapacity", statCollection, 0);
    }

    internal static StatisticAdapter<float> JumpDistanceMultiplier(this StatCollection statCollection)
    {
        return new("JumpDistanceMultiplier", statCollection, 1);
    }

    internal static StatisticAdapter<float> WalkSpeed(this StatCollection statCollection)
    {
        return new("WalkSpeed", statCollection, 0);
    }

    internal static StatisticAdapter<float> RunSpeed(this StatCollection statCollection)
    {
        return new("RunSpeed", statCollection, 0);
    }

    internal static StatisticAdapter<int> HeatSinkCapacity(this StatCollection statCollection)
    {
        return new("HeatSinkCapacity", statCollection, 0);
    }

    internal static StatisticAdapter<float> HeatGenerated(this StatCollection statCollection)
    {
        return new("HeatGenerated", statCollection, 0);
    }

    internal static StatisticAdapter<float> WeaponHeatMultiplier(this StatCollection statCollection)
    {
        return new("WeaponHeatMultiplier", statCollection, 1);
    }

    internal static StatisticAdapter<float> JumpHeat(this StatCollection statCollection)
    {
        return new("JumpHeat", statCollection, 0);
    }

    internal static StatisticAdapter<int> MaxHeat(this StatCollection statCollection)
    {
        return new("MaxHeat", statCollection, MechStatisticsRules.Combat.Heat.MaxHeat);
    }

    internal static StatisticAdapter<int> OverheatLevel(this StatCollection statCollection)
    {
        return new("OverheatLevel", statCollection, (int)(MechStatisticsRules.Combat.Heat.OverheatLevel * MechStatisticsRules.Combat.Heat.MaxHeat));
    }
}