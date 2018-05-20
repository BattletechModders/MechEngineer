using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechLabMechInfoWidget), "GetTotalHardpoints")]
    public static class EngineJumpJetMechLabInfoWidget
    {
        // only allow one engine part per specific location
        public static void Postfix(MechLabMechInfoWidget __instance)
        {
            try
            {
                var adapter = new MechLabMechInfoWidgetAdapter(__instance);
                var engine = adapter.mechLab.activeMechDef.Inventory
                    .Select(x => Engine.MainEngineFromDef(x.Def))
                    .FirstOrDefault(x => x != null);

                var current = adapter.mechLab.headWidget.currentJumpjetCount
                              + adapter.mechLab.centerTorsoWidget.currentJumpjetCount
                              + adapter.mechLab.leftTorsoWidget.currentJumpjetCount
                              + adapter.mechLab.rightTorsoWidget.currentJumpjetCount
                              + adapter.mechLab.leftArmWidget.currentJumpjetCount
                              + adapter.mechLab.rightArmWidget.currentJumpjetCount
                              + adapter.mechLab.leftLegWidget.currentJumpjetCount
                              + adapter.mechLab.rightLegWidget.currentJumpjetCount;


                if (engine == null)
                {
                    adapter.hardpoints[4].SetData(WeaponCategory.AMS, string.Format("{0} / {1}", current, 0));
                    return;
                }

                __instance.totalJumpjets = Control.calc.CalcJumpJetCount(engine, adapter.mechLab.activeMechDef.Chassis.Tonnage);
                adapter.hardpoints[4].SetData(WeaponCategory.AMS, string.Format("{0} / {1}", current, __instance.totalJumpjets));
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}