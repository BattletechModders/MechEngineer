using System;
using System.Linq;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.OnMaxArmor))]
public static class MechLabPanel_OnMaxArmor_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabPanel __instance, MechLabMechInfoWidget ___mechInfoWidget, MechLabItemSlotElement ___dragItem)
    {
        try
        {
            if (__instance.Initialized && ___dragItem == null && !LocationExtensions.ChassisLocationList.Any(location => __instance.GetLocationWidget(location).IsDestroyed))
            {
                ArmorMaximizerHandler.OnMaxArmor(__instance, ___mechInfoWidget);
                return false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
        return true;
    }
}
