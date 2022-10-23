using CustomComponents;
using MechEngineer.Features.Engines.Handler;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.Engines;

internal class EngineFeature : Feature<EngineSettings>
{
    internal static readonly EngineFeature Shared = new();

    internal override EngineSettings Settings => Control.Settings.Engine;

    internal static EngineSettings settings => Shared.Settings;

    protected override void SetupFeatureLoaded()
    {
        HPHandler.GetJumpJetMaxByChassisDef = _ => -1;
        HPHandler.GetJumpJetStatsByMechDef = def =>
        {
            if (def == null)
            {
                return (-1, -1);
            }

            var stats = new MechDefMovementStatistics(def);
            return (stats.JumpJetCount, stats.JumpJetMaxCount);
        };
        Validator.RegisterMechValidator(EngineValidation.Shared.CCValidation.ValidateMech, EngineValidation.Shared.CCValidation.ValidateMechCanBeFielded);
    }
}