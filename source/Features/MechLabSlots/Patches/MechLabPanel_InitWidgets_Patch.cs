using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.InitWidgets))]
public static class MechLabPanel_InitWidgets_Patch
{
    public static void Postfix(MechLabPanel __instance)
    {
        try
        {
            MechLabFixWidgetLayouts.FixMechLabLayouts(__instance);
            CustomWidgetsFixMechLab.Setup(__instance);
            MechLabMoveUIElements.MoveMechUIElements(__instance);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}