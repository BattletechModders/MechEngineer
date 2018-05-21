using System;
using System.Linq;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechLabMechInfoWidget), "CalculateTonnage")]
    public static class StructArmorMechInfoWidgetCalculateTonnagePatch
    {
        // endo-steel and ferros-fibrous calculations for mechlab info widget
        public static void Postfix(MechLabMechInfoWidget __instance)
        {
            try
            {
                var adapter = new MechLabMechInfoWidgetAdapter(__instance);
                var mechLab = adapter.mechLab;
                if (mechLab == null)
                {
                    return;
                }
                var mechDef = mechLab.activeMechDef;
                if (mechDef == null)
                {
                    return;
                }

                __instance.currentTonnage -= StructArmorMechStatisticsRulesPatch.WeightSavingsIfEndoSteel(mechDef);
                __instance.currentTonnage -= StructArmorMechStatisticsRulesPatch.WeightSavingsIfFerrosFibrous(mechDef);

                var current = __instance.currentTonnage;
                var max = mechDef.Chassis.Tonnage;

                var left = max - current;
                var uicolor = left >= 0f ? UIColor.WhiteHalf : UIColor.Red;
                var uicolor2 = left >= 0f ? (left > 5f ? UIColor.White : UIColor.Gold) : UIColor.Red;
                adapter.totalTonnage.text = String.Format("{0:###0.##} / {1}", current, max);
                adapter.totalTonnageColor.SetUIColor(uicolor);
                if (left < 0f)
                {
                    left = Mathf.Abs(left);
                    adapter.remainingTonnage.text = String.Format("{0:###0.##} ton{1} overweight", left, left != 1f ? "s" : String.Empty);
                }
                else
                {
                    adapter.remainingTonnage.text = String.Format("{0:###0.##} ton{1} remaining", left, left != 1f ? "s" : String.Empty);
                }
                adapter.remainingTonnageColor.SetUIColor(uicolor2);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}