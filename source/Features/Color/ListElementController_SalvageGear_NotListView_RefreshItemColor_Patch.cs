using BattleTech.UI;
using Harmony;

namespace CustomComponents
{
    [HarmonyPatch(typeof(ListElementController_SalvageGear_NotListView), "RefreshItemColor")]
    public static class ListElementController_SalvageGear_NotListView_RefreshItemColor_Patch
    {
        public static bool Prefix(InventoryItemElement theWidget, ListElementController_SalvageGear_NotListView __instance)
        {
            var comp = __instance.salvageDef.MechComponentDef?.GetComponent<UIColorComponent>();
            if (comp == null)
            {
                return true;
            }

            var uicolor = comp.SlotElementUIColor;
            foreach (var uicolorRefTracker in theWidget.iconBGColors)
            {
                uicolorRefTracker.SetUIColor(uicolor);
            }

            return false;
        }
    }
}