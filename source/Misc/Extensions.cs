using System;
using BattleTech;
using UnityEngine;

namespace MechEngineer
{
    internal static class Extensions
    {
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
}