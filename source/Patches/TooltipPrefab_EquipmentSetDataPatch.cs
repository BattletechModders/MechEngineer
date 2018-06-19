using System;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using DynModLib;
using Harmony;
using TMPro;

namespace MechEngineMod
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
    }

    [HarmonyPatch(typeof(TooltipPrefab_Equipment), "SetData")]
    public static class TooltipPrefab_EquipmentSetDataPatch
    {
        public static WeakReference panelReference = new WeakReference(null);

        public static void Postfix(TooltipPrefab_Equipment __instance, object data)
        {
            try
            {
                if (data == null)
                {
                    return;
                }

                var panel = panelReference.Target as MechLabPanel;
                if (panel == null)
                {
                    return;
                }

                var adapter = new TooltipPrefab_EquipmentAdapter(__instance);

                var mechComponentDef = (MechComponentDef)data;
                EngineTooltip.AdjustTooltip(adapter, panel, mechComponentDef);
                Structure.AdjustTooltip(adapter, panel, mechComponentDef);
                Armor.AdjustTooltip(adapter, panel, mechComponentDef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
        public static class MechLabPanelLoadMechPatch
        {
            public static void Postfix(MechLabPanel __instance)
            {
                try
                {
                    panelReference.Target = __instance;
                }
                catch (Exception e)
                {
                    Control.mod.Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(MechLabPanel), "ExitMechLab")]
        public static class MechLabPanelExitMechLabPatch
        {
            public static void Postfix(MechLabPanel __instance)
            {
                try
                {
                    panelReference.Target = null;
                }
                catch (Exception e)
                {
                    Control.mod.Logger.LogError(e);
                }
            }
        }
    }
}
