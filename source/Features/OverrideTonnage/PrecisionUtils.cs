using System;
using UnityEngine;

namespace MechEngineer.Features.OverrideTonnage;

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

    // helper functions that combines == and < and <= checks
    internal static bool SmallerThan(float a, float b)
    {
        if (Equals(a, b))
        {
            return false;
        }
        return a < b;
    }
    internal static bool SmallerOrEqualsTo(float a, float b)
    {
        if (Equals(a, b))
        {
            return true;
        }
        return a < b;
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