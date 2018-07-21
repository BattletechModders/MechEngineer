using System.Collections.Generic;
using System.Linq;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabPanel), "ValidateLoadout")]
    public static class MechLabPanel_ValidateLoadout_Patch2
    {
        public static void Postfix(MechLabPanel __instance)
        {
            RefreshSlotElementHandler.Shared.RefreshData(__instance);
        }
    }
}