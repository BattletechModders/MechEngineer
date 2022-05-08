using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.OverrideStatTooltips.Helper;

internal static class MechStatUtils
{
    internal static void SetStatValues(float fraction, ref float currentValue, ref float maxValue)
    {
        var minValue = 1f;
        maxValue = 10f;
        currentValue = fraction * (maxValue - minValue) + minValue;
        currentValue = PrecisionUtils.RoundDownToInt(currentValue);
        currentValue = Mathf.Max(currentValue, minValue);
        currentValue = Mathf.Min(currentValue, maxValue);
    }

    internal static float NormalizeToFraction(float value, float minValue, float maxValue)
    {
        var normalizedValue = (value - minValue) / (maxValue - minValue);
        normalizedValue = Mathf.Max(normalizedValue, 0);
        normalizedValue = Mathf.Min(normalizedValue, 1);
        return normalizedValue;
    }
}