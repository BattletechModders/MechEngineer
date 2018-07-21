using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Harmony;

namespace MechEngineer
{
    internal static class EngineMisc
    {
        internal static void InitEffectstats(Mech mech)
        {
            var engine = mech.MechDef.Inventory.GetEngineCoreRef();

            if (engine == null)
            {
                return;
            }

            var tonnage = mech.tonnage;

            var movement = engine.CoreDef.GetMovement(tonnage);

            mech.StatCollection.GetStatistic("WalkSpeed").SetValue(movement.WalkSpeed);
            mech.StatCollection.GetStatistic("RunSpeed").SetValue(movement.RunSpeed);
        }

        internal static EngineMovement GetEngineMovement(this MechDef mechDef)
        {
            var engine = mechDef.Inventory.GetEngineCoreRef();
            return engine?.CoreDef.GetMovement(mechDef.Chassis.Tonnage);
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

            var engine = mechLab.activeMechInventory.GetEngineCoreRef();

            var current = mechLab.headWidget.currentJumpjetCount
                          + mechLab.centerTorsoWidget.currentJumpjetCount
                          + mechLab.leftTorsoWidget.currentJumpjetCount
                          + mechLab.rightTorsoWidget.currentJumpjetCount
                          + mechLab.leftArmWidget.currentJumpjetCount
                          + mechLab.rightArmWidget.currentJumpjetCount
                          + mechLab.leftLegWidget.currentJumpjetCount
                          + mechLab.rightLegWidget.currentJumpjetCount;

            if (engine == null)
            {
                widget.totalJumpjets = 0;
            }
            else
            {
                widget.totalJumpjets = engine.CoreDef.GetMovement(mechLab.activeMechDef.Chassis.Tonnage).JumpJetCount;
            }

            if (hardpoints == null || hardpoints[4] == null)
            {
                return;
            }

            hardpoints[4].SetData(WeaponCategory.AMS, $"{current} / {widget.totalJumpjets}");
        }

        internal static void RefreshAvailability(MechLabInventoryWidget widget, float tonnage)
        {
            if (tonnage <= 0)
            {
                return;
            }

            foreach (var element in widget.localInventory)
            {
                MechComponentDef componentDef;
                if (element.controller != null)
                {
                    componentDef = element.controller.componentDef;
                }
                else if (element.ComponentRef != null)
                {
                    componentDef = element.ComponentRef.Def;
                }
                else
                {
                    continue;
                }

                var engine = componentDef.GetComponent<EngineCoreDef>();
                if (engine == null)
                {
                    continue;
                }

                var movement = engine.GetMovement(tonnage);
                if (movement.Mountable)
                {
                    continue;
                }

                element.gameObject.SetActive(false);
            }
        }
    }
}