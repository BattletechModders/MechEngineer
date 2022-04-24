using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.LoadMech))]
public static class MechLabPanel_LoadMech_Patch
{
    public static void Postfix(MechLabPanel __instance)
    {
        try
        {
            MechLabAutoZoom.LoadMech(__instance);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}