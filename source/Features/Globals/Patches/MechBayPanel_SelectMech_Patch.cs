using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.Globals.Patches;

[HarmonyPatch(typeof(MechBayPanel), nameof(MechBayPanel.SelectMech))]
public static class MechBayPanel_SelectMech_Patch
{
    [HarmonyPostfix]
    public static void Postfix(MechBayPanel __instance)
    {
        try
        {
            Global.ActiveMechBayPanel = __instance;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
