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
        
        internal static float RoundUpStandard(this float @this)
        {
            if (Control.settings.FractionalAccounting)
            {
                return Mathf.Ceil(@this * 1000f) / 1000f;
            }

            return Mathf.Ceil(@this * 2f) / 2f;
        }
        
        internal static float RoundDownStandard(this float @this)
        {
            if (Control.settings.FractionalAccounting)
            {
                return Mathf.Floor(@this * 1000f) / 1000f;
            }

            return Mathf.Floor(@this * 2f) / 2f;
        }

        internal static float RoundBy5(this float @this)
        {
            return (float)Math.Round(@this / 5) * 5;
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