using BattleTech;
using BattleTech.UI;
using MechEngineer.Features.Engines.Helper;
using UnityEngine;

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

            mech.StatCollection.WalkSpeed().Set(movement.WalkMaxSpeed);
            mech.StatCollection.RunSpeed().Set(movement.RunMaxSpeed);
        }

        internal static EngineMovement GetEngineMovement(this MechDef mechDef)
        {
            var engine = mechDef.GetEngine();
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

            var engine = mechLab.GetEngine();

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
                widget.totalJumpjets = engine.CoreDef.GetMovement(mechLab.activeMechDef.Chassis.Tonnage).JumpJetMaxCount;
            }
            widget.totalJumpjets = Mathf.Min(widget.totalJumpjets, mechLab.activeMechDef.Chassis.MaxJumpjets);

            if (hardpoints == null || hardpoints[4] == null)
            {
                return;
            }

            hardpoints[4].SetData(WeaponCategoryEnumeration.GetAMS(), $"{current} / {widget.totalJumpjets}");
        }

        //internal static void RefreshAvailability(MechLabInventoryWidget widget, float tonnage)
        //{
        //    if (tonnage <= 0)
        //    {
        //        return;
        //    }

        //    foreach (var element in widget.localInventory)
        //    {
        //        MechComponentDef componentDef;
        //        if (element.controller != null)
        //        {
        //            componentDef = element.controller.componentDef;
        //        }
        //        else if (element.ComponentRef != null)
        //        {
        //            componentDef = element.ComponentRef.Def;
        //        }
        //        else
        //        {
        //            continue;
        //        }

        //        var engine = componentDef.GetComponent<EngineCoreDef>();
        //        if (engine == null)
        //        {
        //            continue;
        //        }

        //        var movement = engine.GetMovement(tonnage);
        //        if (movement.Mountable)
        //        {
        //            continue;
        //        }

        //        element.gameObject.SetActive(false);
        //    }
        //}
    }
}