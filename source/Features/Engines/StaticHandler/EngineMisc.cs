using BattleTech;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.Engines.StaticHandler;

internal static class EngineMisc
{
    internal static void OverrideInitEffectStats(Mech mech)
    {
        var engine = mech.MechDef.GetEngine();

        if (engine == null)
        {
            return;
        }

        var tonnage = mech.tonnage;

        var movement = engine.CoreDef.GetMovement(tonnage);

        mech.StatCollection.WalkSpeed().SetDefault(movement.WalkSpeed);
        mech.StatCollection.RunSpeed().SetDefault(movement.RunSpeed);
    }

    internal static string GetJumpJetCountText(MechDef mechDef)
    {
        var stats = new MechDefMovementStatistics(mechDef);
        return stats.JumpJetCount == 0 ? $"{stats.JumpJetMaxCount}" : $"{stats.JumpJetCount} / {stats.JumpJetMaxCount}";
    }
}