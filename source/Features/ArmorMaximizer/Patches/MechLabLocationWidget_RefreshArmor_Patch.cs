using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof (MechLabLocationWidget), nameof(MechLabLocationWidget.RefreshArmor))]
public static class MechLabLocationWidget_RefreshArmor_Patch
{
    [HarmonyPostfix]
    public static void Postfix(MechLabLocationWidget __instance)
    {
        try
        {
            ArmorMaximizerHandler.OnRefreshArmor(__instance);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
