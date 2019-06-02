namespace MechEngineer.Features.OverrideTonnage
{
    internal class OverrideTonnageFeature : Feature<OverrideTonnageSettings>
    {
        internal static readonly OverrideTonnageFeature Shared = new OverrideTonnageFeature();

        internal override OverrideTonnageSettings Settings => Control.settings.OverrideTonnage;

        internal static OverrideTonnageSettings settings => Shared.Settings;
    }
}