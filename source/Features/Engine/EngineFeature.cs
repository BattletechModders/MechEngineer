
using CustomComponents;
using MechEngineer.Features.Engine.Handler;

namespace MechEngineer.Features.Engine
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
