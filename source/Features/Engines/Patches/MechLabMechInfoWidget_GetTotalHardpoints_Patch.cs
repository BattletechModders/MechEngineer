using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(MechLabMechInfoWidget), "GetTotalHardpoints")]
    public static class MechLabMechInfoWidget_GetTotalHardpoints_Patch
    {
        public static void Postfix(MechLabPanel ___mechLab, MechLabHardpointElement[] ___hardpoints)
        {
            try
            {
                if (___hardpoints == null || ___hardpoints.Length < 5 || ___mechLab?.activeMechDef == null)
                {
                    return;
                }
                
                ___hardpoints[4].SetData(
                    WeaponCategoryEnumeration.GetAMS(),
                    EngineMisc.GetJumpJetCountText(___mechLab.activeMechDef)
                );
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}