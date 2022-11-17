using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof (MechLabLocationWidget), nameof(MechLabLocationWidget.OnRearArmorSubtract))]
public static class MechLabLocationWidget_OnRearArmorSubtract_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabLocationWidget __instance)
    {
        try
        {
            ArmorMaximizerHandler.OnArmorAddOrSubtract(__instance, true, -1f);
            return false;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
        return true;
    }
}
