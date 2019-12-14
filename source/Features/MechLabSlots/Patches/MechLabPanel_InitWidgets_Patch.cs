using BattleTech.UI;
using Harmony;
using System;

namespace MechEngineer.Features.MechLabSlots.Patches
{
    [HarmonyPatch(typeof(MechLabPanel), "InitWidgets")]
    public static class MechLabPanel_InitWidgets_Patch
    {
        public static void Postfix(MechLabPanel __instance)
        {
            try
            {
                MechLabFixWidgetLayouts.FixWidgetLayouts(__instance);
                MechPropertiesWidget.Setup(__instance);
                MechLabMoveMechRoleInfo.MoveMechRoleInfo(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}