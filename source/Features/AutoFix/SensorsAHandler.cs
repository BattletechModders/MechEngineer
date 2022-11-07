using MechEngineer.Misc;

namespace MechEngineer.Features.AutoFix;

internal class SensorsAHandler
{
    private static readonly Lazier<SensorsAHandler> Lazy = new();
    internal static SensorsAHandler Shared => Lazy.Value;

    private readonly IdentityHelper? identity;

    public SensorsAHandler()
    {
        identity = AutoFixerFeature.settings.SensorsACategorizer;
    }
}