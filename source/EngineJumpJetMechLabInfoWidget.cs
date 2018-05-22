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
                var mechLab = adapter.mechLab;

                if (mechLab == null)
                {
                    return;
                }

                var engine = mechLab.activeMechDef.Inventory
                    .Select(x => Engine.MainEngineFromDef(x.Def))
                    .FirstOrDefault(x => x != null);

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
                    __instance.totalJumpjets = 0;
                }
                else
                {
                    __instance.totalJumpjets = Control.calc.CalcJumpJetCount(engine, adapter.mechLab.activeMechDef.Chassis.Tonnage);
                }
                adapter.hardpoints[4].SetData(WeaponCategory.AMS, string.Format("{0} / {1}", current, __instance.totalJumpjets));
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}