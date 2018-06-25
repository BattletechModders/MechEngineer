using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabMechInfoWidget), "GetTotalHardpoints")]
    public static class MechLabMechInfoWidgetGetTotalHardpointsPatch
    {
        // only allow one engine part per specific location
        public static void Postfix(MechLabMechInfoWidget __instance, MechLabPanel ___mechLab, MechLabHardpointElement[] ___hardpoints)
        {
            try
            {
                EngineMisc.SetJumpJetHardpointCount(__instance, ___mechLab, ___hardpoints);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}