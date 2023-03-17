using System;
using BattleTech;
using BattleTech.UI;
using MechEngineer.Misc;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(
    typeof(MechLabLocationWidget),
    nameof(MechLabLocationWidget.ShowHighlightFrame),
    typeof(MechComponentRef),
    typeof(WeaponDef),
    typeof(bool),
    typeof(bool)
)]
public static class MechLabLocationWidget_ShowHighlightFrame_Patch
{
    [HarmonyBefore(Mods.CC)]
    [HarmonyPriority(Priority.HigherThanNormal)]
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, MechLabLocationWidget __instance, bool isOriginalLocation, ref MechComponentRef? cRef)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            __runOriginal = CustomWidgetsFixMechLab.ShowHighlightFrame(__instance, isOriginalLocation, ref cRef);
            return;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }

        __runOriginal = false;
    }
}
