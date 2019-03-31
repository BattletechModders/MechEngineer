using System;
using BattleTech.UI;
using Harmony;
using UnityEngine.EventSystems;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.OnDrop))]
    public static class MechLabLocationWidget_OnDrop_Patch
    {
        public static bool Prefix(MechLabLocationWidget __instance, PointerEventData eventData)
        {
            try
            {
                if (__instance == MechLabPanel_InitWidgets_Patch.MechPropertiesWidget)
                {
                    var mechLab = (MechLabPanel) __instance.parentDropTarget;
                    mechLab.centerTorsoWidget.OnDrop(eventData);
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return true;
        }
    }
}