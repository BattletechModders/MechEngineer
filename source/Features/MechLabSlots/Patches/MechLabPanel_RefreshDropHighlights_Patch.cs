using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.MechLabSlots.Patches
{
    [HarmonyPatch(typeof(MechLabPanel), "RefreshDropHighlights")]
    public static class MechLabPanel_RefreshDropHighlights_Patch
    {
        public static void Prefix(MechLabLocationWidget __instance, IMechLabDraggableItem item)
        {
            try
            {
                if (item == null)
                {
                    MechLabPanel_InitWidgets_Patch.MechPropertiesWidget.ShowHighlightFrame(false);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}