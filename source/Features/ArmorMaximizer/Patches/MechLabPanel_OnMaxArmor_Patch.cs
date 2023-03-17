using System;
using System.Linq;
using BattleTech.UI;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.OnMaxArmor))]
public static class MechLabPanel_OnMaxArmor_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, MechLabPanel __instance, MechLabMechInfoWidget ___mechInfoWidget, MechLabItemSlotElement ___dragItem)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            if (__instance.Initialized && ___dragItem == null && !LocationExtensions.ChassisLocationList.Any(location => __instance.GetLocationWidget(location).IsDestroyed))
            {
                ArmorMaximizerHandler.OnMaxArmor(__instance, ___mechInfoWidget);
                __runOriginal = false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
