namespace MechEngineer.Features.OverrideTonnage;

internal class OverrideTonnageFeature : Feature<OverrideTonnageSettings>
{
    internal static readonly OverrideTonnageFeature Shared = new();

    internal override OverrideTonnageSettings Settings => Control.Settings.OverrideTonnage;

    internal static OverrideTonnageSettings settings => Shared.Settings;
}