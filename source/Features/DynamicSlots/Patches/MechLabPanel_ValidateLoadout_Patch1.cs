using System;
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
            try
            {
                DynamicSlotHandler.Shared.RefreshData(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}