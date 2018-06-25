using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using TMPro;
using UnityEngine;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabMechInfoWidget), "CalculateTonnage")]
    public static class MechLabMechInfoWidgetCalculateTonnagePatch
    {
        // endo-steel and ferros-fibrous calculations for mechlab info widget
        public static void Postfix(
            MechLabMechInfoWidget __instance,
            MechLabPanel ___mechLab,
            TextMeshProUGUI ___totalTonnage,
            UIColorRefTracker ___totalTonnageColor,
            TextMeshProUGUI ___remainingTonnage,
            UIColorRefTracker ___remainingTonnageColor)
        {
            try
            {
                if (___mechLab == null)
                {
                    return;
                }
                var mechDef = ___mechLab.activeMechDef;
                if (mechDef == null)
                {
                    return;
                }

                __instance.currentTonnage += CalculateTonnageFacade.AdditionalTonnage(mechDef);

                var current = __instance.currentTonnage;
                var max = mechDef.Chassis.Tonnage;

                var left = max - current;
                var uicolor = left >= 0f ? UIColor.WhiteHalf : UIColor.Red;
                var uicolor2 = left >= 0f ? (left > 5f ? UIColor.White : UIColor.Gold) : UIColor.Red;
                ___totalTonnage.text = String.Format("{0:###0.##} / {1}", current, max);
                ___totalTonnageColor.SetUIColor(uicolor);
                if (left < 0f)
                {
                    left = Mathf.Abs(left);
                    ___remainingTonnage.text = String.Format("{0:###0.##} ton{1} overweight", left, left != 1f ? "s" : String.Empty);
                }
                else
                {
                    ___remainingTonnage.text = String.Format("{0:###0.##} ton{1} remaining", left, left != 1f ? "s" : String.Empty);
                }
                ___remainingTonnageColor.SetUIColor(uicolor2);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}