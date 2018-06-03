using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using Harmony;

namespace MechEngineMod
{
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

                var mechComponentDef = (MechComponentDef)data;
                EngineTooltip.AdjustTooltip(__instance, panel, mechComponentDef);
                EndoSteel.AdjustTooltip(__instance, panel, mechComponentDef);
                FerrosFibrous.AdjustTooltip(__instance, panel, mechComponentDef);
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
                    panelReference = new WeakReference(__instance);
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
                    panelReference = new WeakReference(null);
                }
                catch (Exception e)
                {
                    Control.mod.Logger.LogError(e);
                }
            }
        }
    }
}
