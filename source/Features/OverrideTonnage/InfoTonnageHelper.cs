namespace MechEngineer.Features.OverrideTonnage;

internal class InfoTonnageHelper
{
    internal static InfoTonnageHelper KilogramStandard => new(OverrideTonnageFeature.settings.KilogramStandardPrecision);
    internal static InfoTonnageHelper TonnageStandard => new(OverrideTonnageFeature.settings.TonnageStandardPrecision);

    private readonly float _precision;
    private InfoTonnageHelper(float precision)
    {
        _precision = precision;
    }

    private float Round(float value)
    {
        return PrecisionUtils.RoundUp(value, _precision);
    }

    internal bool IsSame(float a, float b)
    {
        return PrecisionUtils.Equals(a, b, _precision / 2);
    }

    internal bool IsSmaller(float a, float b)
    {
        if (IsSame(a, b))
        {
            return false;
        }
        return a < b;
    }

    internal string AsString(float number)
    {
        var rounded = PrecisionUtils.RoundUp(number, _precision);
        return rounded.ToString(OverrideTonnageFeature.settings.MechLabMechInfoWidgetFormat);
    }

    public bool IsSmaller(float a, float b, out float left)
    {
        if (IsSame(a, b))
        {
            left = 0f;
            return false;
        }

        left = Round(a) - Round(b);
        return left < 0;
    }
}