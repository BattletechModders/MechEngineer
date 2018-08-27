using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabInventoryWidget_ListView), nameof(MechLabInventoryWidget_ListView.ApplySorting))]
    internal static class MechLabInventoryWidget_ListView_ApplySorting_Patch
    {
        internal static void Prefix(MechLabInventoryWidget_ListView __instance)
        {
            try
            {
                var adapter = new MechLabInventoryWidget_ListViewAdapter(__instance);

                if (adapter.invertSort)
                {
                    return;
                }

                if (adapter.currentListItemSorter is InventorySorterListComparer)
                {
                    return;
                }

                adapter.currentListItemSorter = new InventorySorterListComparer(adapter.currentListItemSorter.Compare);
                adapter.currentSort = new InventorySorterListComparer(adapter.currentSort).Compare;
                adapter.invertSort = false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}