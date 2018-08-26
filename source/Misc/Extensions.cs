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
        
        internal static float RoundUp(this float @this, float? precision = null)
        {
            return Round(@this, Mathf.Ceil, precision);
        }
        
        internal static float Round(this float @this, Func<float, float> rounder, float? precision = null)
        {
            var precisionFactor = precision ?? Control.settings.FractionalAccountingPrecision;
            return rounder(@this / precisionFactor) * precisionFactor;
        }
    }

    internal static class StringUtils
    {
        internal static bool IsNullOrWhiteSpace(string value)
        {
            if (value == null)
            {
                return true;
            }

            foreach (var v in value)
            {
                if(!char.IsWhiteSpace(v))
                {
                    return false;
                }
            }
            return true;
        }
    }
}