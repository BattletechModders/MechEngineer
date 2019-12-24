using BattleTech;

namespace MechEngineer
{
    internal class StatisticHelper<T>
    {
        internal readonly string Key;
        internal readonly StatCollection StatCollection;

        internal StatisticHelper(string key, StatCollection statCollection)
        {
            Key = key;
            StatCollection = statCollection;
        }

        internal Statistic Create(T defaultValue)
        {
            return StatCollection.AddStatistic(Key, defaultValue);
        }

        internal T Get()
        {
            return StatCollection.GetValue<T>(Key);
        }

        internal void Set(T value)
        {
            StatCollection.GetStatistic(Key).SetValue(value);
        }
    }
}