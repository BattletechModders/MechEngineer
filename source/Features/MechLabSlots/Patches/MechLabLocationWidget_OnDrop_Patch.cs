using System;
using BattleTech.UI;
using UnityEngine.EventSystems;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.OnDrop))]
public static class MechLabLocationWidget_OnDrop_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, MechLabLocationWidget __instance, PointerEventData eventData)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            if (CustomWidgetsFixMechLab.OnDrop(__instance, eventData))
            {
                __runOriginal = false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
