using CustomComponents;
using MechEngineer.Features.Engines.Handler;

namespace MechEngineer.Features.Engines;

internal class EngineFeature : Feature<EngineSettings>
{
    internal static readonly EngineFeature Shared = new();

    internal override EngineSettings Settings => Control.Settings.Engine;

    internal static EngineSettings settings => Shared.Settings;

    protected override void SetupFeatureLoaded()
    {
        Validator.RegisterMechValidator(EngineValidation.Shared.CCValidation.ValidateMech, EngineValidation.Shared.CCValidation.ValidateMechCanBeFielded);
    }
}