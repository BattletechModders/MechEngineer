using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.Globals.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.ExitMechLab))]
public static class MechLabPanel_ExitMechLab_Patch
{
    [HarmonyPostfix]
    public static void Postfix(MechLabPanel __instance)
    {
        try
        {
            Global.ActiveMechLabPanel = null;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
