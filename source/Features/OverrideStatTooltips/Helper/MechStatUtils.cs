using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.OverrideStatTooltips.Helper
{
    internal class MechStatUtils
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
    }
}
