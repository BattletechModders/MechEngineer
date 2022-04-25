namespace MechEngineer.Features.ArmorMaximizer;

internal class ArmorMaximizerFeature : Feature<ArmorMaximizerSettings>
{
    internal static readonly ArmorMaximizerFeature Shared = new();

    internal override ArmorMaximizerSettings Settings => Control.settings.ArmorMaximizer;
}