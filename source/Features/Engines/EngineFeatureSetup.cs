using CustomComponents;
using MechEngineer.Features.Engines.Handler;

namespace MechEngineer.Features.Engines
{
    internal class EngineFeature : Feature
    {
        internal static EngineFeature Shared = new EngineFeature();

        internal override void SetupFeatureLoaded()
        {
            Validator.RegisterMechValidator(EngineValidation.Shared.CCValidation.ValidateMech, EngineValidation.Shared.CCValidation.ValidateMechCanBeFielded);
        }
    }
}
