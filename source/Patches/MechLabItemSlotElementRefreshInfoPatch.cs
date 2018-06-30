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
                {
                    var engineRef = __instance.ComponentRef.GetEngineCoreRef();
                    if (engineRef != null)
                    {
                        ___bonusTextA.text = engineRef.BonusValueA;
                        ___bonusTextB.text = engineRef.BonusValueB;
                    }
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}