using System.Collections.Generic;
using CustomComponents;
using MechEngineer.Misc;

namespace MechEngineer.Features.AutoFix;

internal class SensorsAHandler : IPreProcessor
{
    private static readonly Lazier<SensorsAHandler> Lazy = new();
    internal static SensorsAHandler Shared => Lazy.Value;

    private readonly IdentityHelper? identity;

    public SensorsAHandler()
    {
        identity = AutoFixerFeature.settings.SensorsACategorizer;
    }

    public void PreProcess(object target, Dictionary<string, object> values)
    {
        identity?.PreProcess(target, values);
    }
}