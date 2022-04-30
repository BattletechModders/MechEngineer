using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.OnFrontArmorSubtract))]
public static class MechLabLocationWidget_OnFrontArmorSubtract_Patch
{
    public static bool Prefix(MechLabLocationWidget __instance)
    {
        try
        {
            ArmorMaximizerHandler.HandleArmorUpdate(__instance, false, -1f);
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}
