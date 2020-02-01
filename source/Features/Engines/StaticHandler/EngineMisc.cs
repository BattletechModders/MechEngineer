using BattleTech;
using BattleTech.UI;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.OverrideStatTooltips.Helper;

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
            if (mechLab == null || mechLab.activeMechDef == null || mechLab.activeMechInventory == null)
            {
                return;
            }

            if (mechLab.activeMechDef.Chassis == null)
            {
                return;
            }

            var current = mechLab.headWidget.currentJumpjetCount
                          + mechLab.centerTorsoWidget.currentJumpjetCount
                          + mechLab.leftTorsoWidget.currentJumpjetCount
                          + mechLab.rightTorsoWidget.currentJumpjetCount
                          + mechLab.leftArmWidget.currentJumpjetCount
                          + mechLab.rightArmWidget.currentJumpjetCount
                          + mechLab.leftLegWidget.currentJumpjetCount
                          + mechLab.rightLegWidget.currentJumpjetCount;

            var stats = new MechDefMovementStatistics(mechLab.activeMechDef);
            widget.totalJumpjets = stats.JumpJetCount;

            if (hardpoints == null || hardpoints[4] == null)
            {
                return;
            }

            hardpoints[4].SetData(WeaponCategoryEnumeration.GetAMS(), $"{current} / {widget.totalJumpjets}");
        }
    }
}