using System;
using UnityEngine;

namespace MechEngineer.Features.OverrideTonnage
{
    internal static class PrecisionUtils
    {
        internal static float RoundUpOverridableDefault(float value, float overridablePrecision)
        {
            return Round(value, Mathf.Ceil, OverrideTonnageFeature.settings.FractionalAccountingPrecision ?? overridablePrecision);
        }

        internal static float RoundUp(float value, float? precision = null)
        {
            return Round(value, Mathf.Ceil, precision);
        }
        
        internal static float Round(float value, Func<float, float> rounder, float? precision)
        {
            var precisionFactor = precision ?? OverrideTonnageFeature.settings.FractionalAccountingPrecision ?? 0.5f;
            return rounder(value / precisionFactor) * precisionFactor;
        }
    }
}