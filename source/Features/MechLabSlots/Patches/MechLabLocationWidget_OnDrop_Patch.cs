using System;
using BattleTech.UI;
using Harmony;
using UnityEngine.EventSystems;

namespace MechEngineer.Features.MechLabSlots.Patches
{
    [HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.OnDrop))]
    public static class MechLabLocationWidget_OnDrop_Patch
    {
        public static bool Prefix(MechLabLocationWidget __instance, PointerEventData eventData)
        {
            try
            {
                if (MechPropertiesWidget.OnDrop(__instance, eventData))
                {
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