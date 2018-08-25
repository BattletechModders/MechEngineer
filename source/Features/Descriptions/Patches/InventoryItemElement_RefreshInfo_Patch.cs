using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(ListElementController_BASE_NotListView), nameof(ListElementController_BASE_NotListView.SetComponentTooltipData))]
    internal static class ListElementController_BASE_NotListView_SetComponentTooltipData_Patch
    {
        internal static void Postfix(ListElementController_BASE_NotListView __instance)
        {
            try
            {
                OverrideDescriptionsHandler.Shared.AdjustInventoryElement(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}