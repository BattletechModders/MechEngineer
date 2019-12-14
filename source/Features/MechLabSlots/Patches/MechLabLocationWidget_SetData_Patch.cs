using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MechEngineer.Features.MechLabSlots.Patches
{
    [HarmonyPatch(typeof(MechLabLocationWidget), "SetData")]
    public static class MechLabLocationWidget_SetData_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(Transform), nameof(Transform.SetParent), new []{typeof(Transform), typeof(bool)}),
                AccessTools.Method(typeof(MechPropertiesWidget), nameof(MechPropertiesWidget.OnAdditem_SetParent))
            );
        }

        public static void Postfix(MechLabLocationWidget __instance, int ___maxSlots, LocationLoadoutDef ___loadout)
        {
            try
            {
                MechLabSlotsFixer.FixSlots(__instance, ___maxSlots, ___loadout);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}