using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.Globals.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.LoadMech))]
public static class MechLabPanel_LoadMech_Patch
{
    [HarmonyPostfix]
    public static void Postfix(MechLabPanel __instance)
    {
        try
        {
            Global.ActiveMechLabPanel = __instance;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}