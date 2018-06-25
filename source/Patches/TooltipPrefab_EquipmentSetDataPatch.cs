using System;
using BattleTech;
using BattleTech.UI.Tooltips;
using DynModLib;
using Harmony;
using TMPro;

namespace MechEngineer
{
    public class TooltipPrefab_EquipmentAdapter : Adapter<TooltipPrefab_Equipment>
    {
        public TooltipPrefab_EquipmentAdapter(TooltipPrefab_Equipment instance) : base(instance)
        {
        }
        
        public TextMeshProUGUI bonusesText
        {
            get { return traverse.Field("bonusesText").GetValue<TextMeshProUGUI>(); }
        }
        
        public TextMeshProUGUI detailText
        {
            get { return traverse.Field("detailText").GetValue<TextMeshProUGUI>(); }
        }

        public TextMeshProUGUI tonnageText
        {
            get { return traverse.Field("tonnageText").GetValue<TextMeshProUGUI>(); }
        }
    }

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

                var mechComponentDef = (MechComponentDef)data;
                EngineTooltip.AdjustTooltip(adapter, panel, mechComponentDef);
                ArmorStructure.AdjustTooltip(adapter, panel, mechComponentDef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
