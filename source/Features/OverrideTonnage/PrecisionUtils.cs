using System;
using UnityEngine;

namespace MechEngineer.Features.OverrideTonnage
{
    internal static class PrecisionUtils
    {
        internal static bool Equals(float a, float b)
        {
            return Equals(a, b, OverrideTonnageFeature.settings.PrecisionEpsilon);
        }
        internal static bool Equals(float a, float b, float epsilon)
        {
            return Mathf.Abs(a - b) < epsilon;
        }

        internal static float RoundUp(float value)
        {
            return Round(value, Mathf.Ceil, OverrideTonnageFeature.settings.TonnageStandardPrecision);
        }

        internal static float RoundUp(float value, float precision)
        {
            return Round(value - OverrideTonnageFeature.settings.PrecisionEpsilon, Mathf.Ceil, precision);
        }

        internal static int RoundUpToInt(float value)
        {
            return Mathf.CeilToInt(value - OverrideTonnageFeature.settings.PrecisionEpsilon);
        }

        internal static float RoundDown(float value, float precision)
        {
            return Round(value + OverrideTonnageFeature.settings.PrecisionEpsilon, Mathf.Floor, precision);
        }

        internal static int RoundDownToInt(float value)
        {
            return Mathf.FloorToInt(value + OverrideTonnageFeature.settings.PrecisionEpsilon);
        }
        
        private static float Round(float value, Func<float, float> rounder, float precision)
        {
            return rounder(value / precision) * precision;
        }
    }
}