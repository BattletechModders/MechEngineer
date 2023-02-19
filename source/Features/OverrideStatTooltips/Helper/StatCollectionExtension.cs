using BattleTech;
using MechEngineer.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Helper;

internal static class StatCollectionExtension
{
    internal static StatisticAdapter<float> DamagePerShot(this StatCollection statCollection, float baseValue)
    {
        return new("DamagePerShot", statCollection, baseValue);
    }

    internal static StatisticAdapter<int> ShotsWhenFired(this StatCollection statCollection, int baseValue)
    {
        return new("ShotsWhenFired", statCollection, baseValue);
    }

    internal static StatisticAdapter<float> Instability(this StatCollection statCollection, float baseValue)
    {
        return new("Instability", statCollection, baseValue);
    }

    internal static StatisticAdapter<float> AccuracyModifier(this StatCollection statCollection, float baseValue)
    {
        return new("AccuracyModifier", statCollection, baseValue);
    }

    internal static StatisticAdapter<float> StructureDamagePerShot(this StatCollection statCollection, float baseValue)
    {
        return new("StructureDamagePerShot", statCollection, baseValue);
    }

    internal static StatisticAdapter<float> HeatDamageModifier(this StatCollection statCollection, float baseValue)
    {
        return new("HeatDamageModifier", statCollection, baseValue);
    }

    internal static StatisticAdapter<float> HeatDamagePerShot(this StatCollection statCollection, float baseValue)
    {
        return new("HeatDamagePerShot", statCollection, baseValue);
    }
}