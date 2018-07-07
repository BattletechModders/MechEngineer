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

            Control.calc.CalcSpeeds(engine.CoreDef, tonnage, out var walkSpeed, out var runSpeed);

            mech.StatCollection.GetStatistic("WalkSpeed").SetValue(walkSpeed);
            mech.StatCollection.GetStatistic("RunSpeed").SetValue(runSpeed);
        }

        internal static bool CalculateMovementStat(MechDef mechDef, ref float walkSpeed, ref float runSpeed, ref float TTWalkSpeed)
        {
            var engine = mechDef.Inventory.GetEngineCoreRef();
            if (engine == null)
            {
                return false;
            }

            var maxTonnage = mechDef.Chassis.Tonnage;
            Control.calc.CalcSpeeds(engine.CoreDef, maxTonnage, out walkSpeed, out runSpeed);
            TTWalkSpeed = Control.calc.CalcMovementPoints(engine.CoreDef, maxTonnage);

            return true;
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
                widget.totalJumpjets = Control.calc.CalcJumpJetCount(engine.CoreDef, mechLab.activeMechDef.Chassis.Tonnage);
            }

            if (hardpoints == null || hardpoints[4] == null)
            {
                return;
            }

            hardpoints[4].SetData(WeaponCategory.AMS, string.Format("{0} / {1}", current, widget.totalJumpjets));
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

                if (Control.calc.CalcAvailability(engine, tonnage))
                {
                    continue;
                }

                element.gameObject.SetActive(false);
            }
        }
    }
}