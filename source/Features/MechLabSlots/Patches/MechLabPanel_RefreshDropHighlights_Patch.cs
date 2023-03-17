using System;
using BattleTech.UI;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.RefreshDropHighlights))]
public static class MechLabPanel_RefreshDropHighlights_Patch
{
    public static void Prefix(ref bool __runOriginal, MechLabLocationWidget __instance, IMechLabDraggableItem item)
    {
        if (!__runOriginal)
        {
            return;
        }

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
