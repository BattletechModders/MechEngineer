using System.Collections.Generic;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace MechEngineer.Features.MechLabSlots.Patches
{
    [HarmonyPatch(typeof(MechLabLocationWidget), "OnAddItem")]
    public static class MechLabLocationWidget_OnAddItem_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(Transform), nameof(Transform.SetParent), new []{typeof(Transform), typeof(bool)}),
                AccessTools.Method(typeof(MechPropertiesWidget), nameof(MechPropertiesWidget.OnAdditem_SetParent))
            );
        }
    }
}