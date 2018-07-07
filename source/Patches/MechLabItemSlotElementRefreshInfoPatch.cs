using System;
using BattleTech.UI;
using Harmony;
using TMPro;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabItemSlotElement), "RefreshInfo")]
    internal static class MechLabItemSlotElementRefreshInfoPatch
    {
        internal static void Postfix(this MechLabItemSlotElement __instance, TextMeshProUGUI ___nameText, TextMeshProUGUI ___bonusTextA, TextMeshProUGUI ___bonusTextB)
        {
            try
            {
                EngineCoreRefHandler.Shared.MechLabItemRefreshInfo(__instance);
                WeightSavingsHandler.Shared.MechLabItemRefreshInfo(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}