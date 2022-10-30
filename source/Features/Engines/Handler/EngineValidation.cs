using BattleTech;
using MechEngineer.Features.OverrideStatTooltips.Helper;
using MechEngineer.Helper;
using MechEngineer.Misc;

namespace MechEngineer.Features.Engines.Handler;

internal class EngineValidation : IValidateMech
{
    internal static readonly EngineValidation Shared = new();
    internal readonly CCValidationAdapter CCValidation;

    internal EngineValidation()
    {
        CCValidation = new CCValidationAdapter(this);
    }

    public void ValidateMech(MechDef mechDef, Errors errors)
    {
        var stats = new MechDefMovementStatistics(mechDef);
        var engine = stats.Engine;
        if (engine == null)
        {
            return;
        }

        {
            var count = stats.JumpJetCount;
            var max = stats.JumpJetMaxCount;

            if (count > max)
            {
                if (errors.Add(MechValidationType.InvalidJumpjets, $"JUMP JETS: This 'Mech mounts too many jumpjets ({count} / {max})"))
                {
                    return;
                }
            }
        }
    }
}