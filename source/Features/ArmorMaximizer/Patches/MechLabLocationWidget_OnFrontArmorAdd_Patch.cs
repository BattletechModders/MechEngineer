using System;
using BattleTech.UI;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.OnFrontArmorAdd))]
public static class MechLabLocationWidget_OnFrontArmorAdd_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, MechLabLocationWidget __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            ArmorMaximizerHandler.OnArmorAddOrSubtract(__instance, false, +1f);
            __runOriginal = false;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
