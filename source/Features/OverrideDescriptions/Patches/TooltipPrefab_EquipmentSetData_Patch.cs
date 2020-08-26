using System;
using BattleTech;
using BattleTech.UI.Tooltips;
using Harmony;

namespace MechEngineer.Features.OverrideDescriptions.Patches
{
    [HarmonyPatch(typeof(TooltipPrefab_Equipment), "SetData")]
    public static class TooltipPrefab_EquipmentSetData_Patch
    {
        public static void Postfix(TooltipPrefab_Equipment __instance, object data)
        {
            try
            {
                var adapter = new TooltipPrefab_EquipmentAdapter(__instance);
                adapter.ShowBonuses = true;

                if (data is MechComponentDef def)
                {
                    OverrideDescriptionsFeature.Shared.AdjustTooltipEquipment(__instance, def);
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}