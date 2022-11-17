using System;
using BattleTech.UI;
using Harmony;
using UnityEngine.EventSystems;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.OnDrop))]
public static class MechLabLocationWidget_OnDrop_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabLocationWidget __instance, PointerEventData eventData)
    {
        try
        {
            if (CustomWidgetsFixMechLab.OnDrop(__instance, eventData))
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }

        return true;
    }
}
