using System;
using BattleTech;
using BattleTech.UI.Tooltips;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(TooltipPrefab_Equipment), "SetData")]
    public static class TooltipPrefab_EquipmentSetDataPatch
    {
        public static void Postfix(TooltipPrefab_Equipment __instance, object data)
        {
            try
            {
                if (data == null)
                {
                    return;
                }

                var panel = MechLab.Current;
                if (panel == null)
                {
                    return;
                }

                var adapter = new TooltipPrefab_EquipmentAdapter(__instance);

                var mechComponentDef = (MechComponentDef) data;
                EngineHandler.Shared.AdjustTooltip(adapter, panel, mechComponentDef);
                WeightSavingsHandler.Shared.AdjustTooltip(adapter, panel, mechComponentDef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}