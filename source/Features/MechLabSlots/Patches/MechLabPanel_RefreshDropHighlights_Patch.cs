using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.RefreshDropHighlights))]
public static class MechLabPanel_RefreshDropHighlights_Patch
{
    public static void Prefix(MechLabLocationWidget __instance, IMechLabDraggableItem item)
    {
        try
        {
            CustomWidgetsFixMechLab.RefreshDropHighlights(__instance, item);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
