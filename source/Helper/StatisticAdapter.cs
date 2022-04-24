using System;
using BattleTech;
using Variant = BattleTech.Variant;

namespace MechEngineer.Helper;

internal class StatisticAdapter<T>
{
    internal readonly string Key;
    internal readonly StatCollection StatCollection;
    internal readonly T DefaultValue;

    internal StatisticAdapter(string key, StatCollection statCollection, T defaultValue)
    {
        Key = key;
        StatCollection = statCollection;
        DefaultValue = defaultValue;
    }

    internal Statistic Create()
    {
        return StatCollection.AddStatistic(Key, DefaultValue);
    }

    internal Statistic CreateWithDefault(T defaultValue)
    {
        return StatCollection.AddStatistic(Key, defaultValue);
    }

    internal T Get()
    {
        return StatCollection.GetValue<T>(Key);
    }

    internal void SetValue(T value)
    {
        StatCollection.GetStatistic(Key).SetValue(value);
    }

    internal void SetDefault(T value, bool reset = true)
    {
        var stat = StatCollection.GetStatistic(Key);
        stat.defaultValue.objVal = value;
        if (reset)
        {
            stat.Reset();
        }
    }

    internal void CreateIfMissing()
    {
        if (!StatCollection.ContainsStatistic(Key))
        {
            Create();
        }
    }

    internal void Modify(EffectData effectData)
    {
        var modType = Type.GetType(effectData.statisticData.modType);
        var modVariant = new Variant(modType);
        modVariant.statName = effectData.statisticData.statName;
        modVariant.SetValue(effectData.statisticData.modValue);
        StatCollection.ModifyStatistic(null, -1, effectData.statisticData.statName, effectData.statisticData.operation, modVariant, -1, true);
    }

    internal StatisticEffectData CreateStatisticData(StatCollection.StatOperation op, T value)
    {
        return new()
        {
            statName = Key,
            operation = op,
            modValue = value.ToString(),
            modType = value.GetType().ToString()
        };
    }
}