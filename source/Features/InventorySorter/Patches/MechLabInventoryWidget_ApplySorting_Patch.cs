using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.ApplySorting))]
    internal static class MechLabInventoryWidget_ApplySorting_Patch
    {
        private static Comparison<InventoryItemElement_NotListView> currentSort;
        internal static void Prefix(MechLabInventoryWidget __instance)
        {
            try
            {
                var adapter = new MechLabInventoryWidgetAdapter(__instance);

                if (adapter.currentSort == null)
                {
                    return;
                }

                if (adapter.currentSort == currentSort)
                {
                    return;
                }

                currentSort = new InventorySorterNotListComparer(adapter.currentSort).Compare;
                adapter.currentSort = currentSort;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}