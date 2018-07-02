using System;
using BattleTech;
using UnityEngine;

namespace MechEngineer
{
    internal static class Extensions
    {
        internal static void PerformOperation(this StatCollection collection, Statistic statistic, StatisticEffectData data)
        {
            var type = Type.GetType(data.modType);
            var variant = new Variant(type);
            variant.SetValue(data.modValue);
            variant.statName = data.statName;
            collection.PerformOperation(statistic, data.operation, variant);
        }

        internal static float RoundStandard(this float @this)
        {
            if (Control.settings.FractionalAccounting)
            {
                return Mathf.Round(@this * 1000) / 1000;
            }

            return Mathf.Round(@this * 2) / 2;
        }

        internal static float RoundBy5(this float @this)
        {
            return Mathf.Round(@this / 5) * 5;
        }
    }
}