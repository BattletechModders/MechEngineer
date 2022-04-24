using MechEngineer.Features.Engines;
using MechEngineer.Features.OverrideTonnage;

namespace MechEngineer.Features.Globals;

internal class GlobalsFeature : Feature<GlobalsSettings>
{
    internal static readonly GlobalsFeature Shared = new();

    internal override bool Enabled => Settings.Enabled && (OverrideTonnageFeature.Shared.Enabled || EngineFeature.Shared.Enabled);

    internal override GlobalsSettings Settings => Control.settings.Globals;
}