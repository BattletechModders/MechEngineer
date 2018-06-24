using System;
using BattleTech.UI;
using Harmony;
using TMPro;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechLabItemSlotElement), "RefreshInfo")]
    internal static class MechLabItemSlotElementRefreshInfoPatch
    {
        internal static void Postfix(this MechLabItemSlotElement __instance, TextMeshProUGUI ___nameText, TextMeshProUGUI ___bonusTextA, TextMeshProUGUI ___bonusTextB)
        {
            try
            {
                var engineRef = __instance.ComponentRef.GetEngineCoreRef(null);
                if (engineRef == null)
                {
                    return;
                }

                ___bonusTextA.text = engineRef.BonusValueA;
                ___bonusTextB.text = engineRef.BonusValueB;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}