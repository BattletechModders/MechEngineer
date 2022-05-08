using System;
using System.Linq;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.OnStripArmor))]
public static class MechLabPanel_OnStripArmor_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabPanel __instance, MechLabMechInfoWidget ___mechInfoWidget, MechLabItemSlotElement ___dragItem)
    {
        try
        {
            if (__instance.Initialized && ___dragItem == null && !LocationExtensions.ChassisLocationList.Any(location => __instance.GetLocationWidget(location).IsDestroyed))
            {
                ArmorMaximizerHandler.OnStripArmor(__instance);
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