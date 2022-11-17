using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.InitWidgets))]
public static class MechLabPanel_InitWidgets_Patch
{
    [HarmonyPostfix]
    public static void Postfix(MechLabPanel __instance)
    {
        try
        {
            MechLabLayoutUtils.FixMechLabLayouts(__instance);
            CustomWidgetsFixMechLab.Setup(__instance);
            MechLabMoveUIElements.MoveMechUIElements(__instance);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
