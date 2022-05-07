namespace MechEngineer.Features.Performance;

internal class PerformanceFeature : Feature<PerformanceSettings>
{
    internal static readonly PerformanceFeature Shared = new();

    internal override PerformanceSettings Settings => Control.Settings.Performance;

    internal static PerformanceSettings settings => Shared.Settings;
}