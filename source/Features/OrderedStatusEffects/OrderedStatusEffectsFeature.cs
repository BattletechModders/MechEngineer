using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;
using static BattleTech.StatCollection;

namespace MechEngineer.Features.OrderedStatusEffects;

internal class OrderedStatusEffectsFeature : Feature<OrderedStatusEffectsSettings>
{
    internal static readonly OrderedStatusEffectsFeature Shared = new();

    internal override OrderedStatusEffectsSettings Settings => Control.settings.OrderedStatusEffects;

    internal void SortEffectDataList(List<EffectData> list)
    {
        list.Sort((a, b) => GetOrderSet99(a.statisticData.operation).CompareTo(GetOrderSet99(b.statisticData.operation)));
    }

    private int GetOrderSet99(StatOperation op)
    {
        if (op == StatOperation.Set)
        {
            return 99;
        }
        return GetOrder(op);
    }

    internal bool SortLatestHistory(List<StatHistory.Event> historyList, int statUID)
    {
        var modified = false;
        var lastIndex = 0;
        var lastOrder = 0;
        StatHistory.Event lastEvent = null;
        // from newest (last) to oldest (current)
        for (var currentIndex = historyList.Count - 1; currentIndex >= 0; currentIndex--)
        {
            var currentEvent = historyList[currentIndex];
            if (currentEvent.statUID != statUID)
            {
                continue;
            }

            var currentOrder = GetOrder(currentEvent.operation);
            if (currentOrder < 0)
            {
                break; // unsortable operation found
            }

            if (lastEvent != null)
            {
                if (currentOrder > lastOrder)
                {
                    Control.Logger.Debug?.Log($"sorting statName={currentEvent.statName} coperation={currentEvent.operation} loperation={lastEvent.operation}");
                    historyList[currentIndex] = lastEvent;
                    historyList[lastIndex] = currentEvent;

                    lastIndex = currentIndex;
                    modified = true;
                }
                else
                {
                    break; // already sorted
                }
            }
            else
            {
                lastIndex = currentIndex;
                lastOrder = currentOrder;
                lastEvent = currentEvent;
            }
        }
        return modified;
    }

    internal void ModifyStatisticPostfix(StatCollection statCollection, string statName)
    {
        if (Settings.FilterStatistics != null && !Settings.FilterStatistics.Contains(statName))
        {
            return;
        }
        if (Settings.OtherStatisticsRequired != null && !Settings.OtherStatisticsRequired.Any(statCollection.ContainsStatistic))
        {
            return;
        }
        var stat = statCollection.GetStatistic(statName);
        var historyList = statCollection.History.historyList;
        if (SortLatestHistory(historyList, stat.uid))
        {
            statCollection.RefreshStatistic(stat);
        }
    }

    private int GetOrder(StatOperation op)
    {
        return Array.IndexOf(Settings.Order, op);
    }
}