using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
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
    public static bool Prefix(MechLabLocationWidget __instance, bool isOriginalLocation, ref MechComponentRef cRef)
    {
        try
        {
            return CustomWidgetsFixMechLab.ShowHighlightFrame(__instance, isOriginalLocation, ref cRef);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }

        return false;
    }
}