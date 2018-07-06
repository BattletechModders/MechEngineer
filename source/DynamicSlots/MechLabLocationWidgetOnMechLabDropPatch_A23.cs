using System.Collections.Generic;
using System.Linq;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabLocationWidget), "OnMechLabDrop")]
    public static class MechLabLocationWidgetOnMechLabDropPatch_A23
    {
        [HarmonyPriority(Priority.High)]
        public static void Prefix(MechLabLocationWidget __instance, MechLabPanel ___mechLab, ref int ___usedSlots, int ___maxSlots)
        {
            var slots = new MechDefSlots(___mechLab.activeMechDef);
            var leftOver = Mathf.Max(slots.Total - (slots.Used + slots.Reserved), 0);
            var free = Mathf.Min(leftOver, ___maxSlots - ___usedSlots);

            ___usedSlots = ___maxSlots - free;
        }

        public static void Postfix(List<MechLabItemSlotElement> ___localInventory, MechLabPanel ___mechLab, ref int ___usedSlots)
        {
            ___usedSlots = MechDefSlots.GetUsedSlots(___localInventory.Select(s => s.ComponentRef));
            DynamicSlotController.Shared.RefreshData(___mechLab.activeMechDef);
        }
    }
}