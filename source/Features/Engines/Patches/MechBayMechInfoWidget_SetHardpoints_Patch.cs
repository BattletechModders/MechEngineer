using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using MechEngineer.Features.Engines.StaticHandler;
using TMPro;

namespace MechEngineer.Features.Engines.Patches
{
    //[HarmonyPatch(typeof(MechBayMechInfoWidget), "SetHardpoints")]
    internal static class MechBayMechInfoWidget_SetHardpoints_Patch
    {
        internal static void Postfix(MechDef ___selectedMech, TextMeshProUGUI ___jumpjetHardpointText)
        {
            try
            {
                if (___selectedMech == null)
                {
                    return;
                }
                ___jumpjetHardpointText.SetText(EngineMisc.GetJumpJetCountText(___selectedMech));
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}
