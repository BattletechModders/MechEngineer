using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.OnMaxArmor))]
public static class MechLabPanel_OnMaxArmor_Patch
{
    public static bool Prefix(MechLabPanel __instance, MechLabMechInfoWidget ___mechInfoWidget, MechLabItemSlotElement ___dragItem)
    {
        try
        {
            if (__instance.Initialized && ___dragItem == null)
            {
                ArmorMaximizerHandler.OnMaxArmor(__instance, ___mechInfoWidget);
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}