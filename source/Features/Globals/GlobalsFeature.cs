using MechEngineer.Features.Engines;
using MechEngineer.Features.OverrideTonnage;

namespace MechEngineer.Features.Globals
{
    internal class GlobalsFeature : Feature<BaseSettings>
    {
        internal static readonly GlobalsFeature Shared = new GlobalsFeature();

        internal override bool Enabled => OverrideTonnageFeature.Shared.Enabled || EngineFeature.Shared.Enabled;

        internal override BaseSettings Settings => new BaseSettings();
    }
}