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
                {
                    var engineRef = __instance.ComponentRef.GetEngineCoreRef(null);
                    if (engineRef != null)
                    {
                        ___bonusTextA.text = engineRef.BonusValueA;
                        ___bonusTextB.text = engineRef.BonusValueB;
                    }
                }

                // didn't work
                //Control.mod.Logger.LogDebug("__instance.ComponentRef.Def.IsEngineCenterSlots() = " + __instance.ComponentRef.Def.IsEngineCenterSlots());
                //if (__instance.ComponentRef.Def.IsEngineCenterSlots())
                //{
                //    var mechLab = MechLab.Current;
                //    Control.mod.Logger.LogDebug("mechLab = " + mechLab);
                //    if (mechLab != null)
                //    {
                //        var engineRef = mechLab.activeMechInventory.GetEngineCoreRef();
                        
                //        Control.mod.Logger.LogDebug("engineRef = " + engineRef);
                //        if (engineRef != null)
                //        {
                //            var changes = engineRef.TonnageChanges;
                            
                //            Control.mod.Logger.LogDebug("changes = " + changes);
                //            if (changes > 0 || changes < 0)
                //            {
                //                if (engineRef.TonnageChanges > 0)
                //                {
                //                    ___bonusTextA.text = "+ " + engineRef.TonnageChanges + " Tons";
                //                }
                //                else
                //                {
                //                    ___bonusTextA.text = "- " + -engineRef.TonnageChanges + " Tons";
                //                }
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}