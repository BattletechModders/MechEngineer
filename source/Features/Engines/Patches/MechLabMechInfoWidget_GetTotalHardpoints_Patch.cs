using System;
using BattleTech.UI;
using Harmony;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(MechLabMechInfoWidget), "GetTotalHardpoints")]
    public static class MechLabMechInfoWidget_GetTotalHardpoints_Patch
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
                Control.Logger.Error.Log(e);
            }
        }
    }
}