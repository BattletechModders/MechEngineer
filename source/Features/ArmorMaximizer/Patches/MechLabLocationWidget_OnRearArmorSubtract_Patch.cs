using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof (MechLabLocationWidget), nameof(MechLabLocationWidget.OnRearArmorSubtract))]
public static class MechLabLocationWidget_OnRearArmorSubtract_Patch
{
    public static bool Prefix(MechLabLocationWidget __instance)
    {
        try
        {
            ArmorMaximizerHandler.OnArmorAddOrSubtract(__instance, true, -1f);
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}
