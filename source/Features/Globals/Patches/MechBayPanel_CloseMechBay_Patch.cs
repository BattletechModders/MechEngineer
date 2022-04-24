using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.Globals.Patches;

[HarmonyPatch(typeof(MechBayPanel), nameof(MechBayPanel.CloseMechBay))]
public static class MechBayPanel_CloseMechBay_Patch
{
    public static void Postfix(MechBayPanel __instance)
    {
        try
        {
            Global.ActiveMechBayPanel = null;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}