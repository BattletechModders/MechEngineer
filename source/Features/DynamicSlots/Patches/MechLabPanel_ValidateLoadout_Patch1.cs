using System.Collections.Generic;
using System.Linq;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabPanel), "ValidateLoadout")]
    public static class MechLabPanel_ValidateLoadout_Patch1
    {
        public static void Postfix(MechLabPanel __instance)
        {
            DynamicSlotHandler.Shared.RefreshData(__instance);
        }
    }
}