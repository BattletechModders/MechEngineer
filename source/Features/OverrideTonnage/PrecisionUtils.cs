using System;
using UnityEngine;

namespace MechEngineer.Features.OverrideTonnage
{
    internal static class PrecisionUtils
    {
        internal static float RoundUp(float value, float? precision = null)
        {
            return Round(value, Mathf.Ceil, precision);
        }
        
        internal static float Round(float value, Func<float, float> rounder, float? precision = null)
        {
            var precisionFactor = precision ?? OverrideTonnageFeature.settings.FractionalAccountingPrecision;
            return rounder(value / precisionFactor) * precisionFactor;
        }
    }
}