using BattleTech;
using BattleTech.UI;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.OverrideStatTooltips.Helper;
using System.Linq;

namespace MechEngineer.Features.Engines.StaticHandler
{
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

            mech.StatCollection.WalkSpeed().Set(movement.WalkSpeed);
            mech.StatCollection.RunSpeed().Set(movement.RunSpeed);
        }

        internal static void SetJumpJetHardpointCount(MechLabMechInfoWidget widget, MechLabPanel mechLab, MechLabHardpointElement[] hardpoints)
        {
            if (hardpoints == null || hardpoints[4] == null)
            {
                return;
            }

            var mechDef = mechLab?.CreateMechDef();
            if (mechDef == null)
            {
                return;
            }

            var stats = new MechDefMovementStatistics(mechDef);
            widget.totalJumpjets = stats.JumpJetMaxCount;

            hardpoints[4].SetData(WeaponCategoryEnumeration.GetAMS(), $"{stats.JumpJetCount} / {stats.JumpJetMaxCount}");
        }
    }
}