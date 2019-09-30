using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MechEngineer.Features.DebugScreenshotMechs
{
    [HarmonyPatch(typeof(SkirmishMechBayPanel), nameof(SkirmishMechBayPanel.OnMechLabCancel))]
    public static class SkirmishMechBayPanel_OnMechLabCancel_Patch
    {
        public static void Postfix(SkirmishMechBayPanel __instance)
        {
            try
            {
                __instance.StartCoroutine(CallBack(__instance));
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
        
        private static IEnumerator<MechDef> mechDefsIterator;
        private static IEnumerator CallBack(SkirmishMechBayPanel panel)
        {
            yield return new WaitForEndOfFrame();
            try
            {
                if (mechDefsIterator == null)
                {
                    mechDefsIterator = panel.allMechs.GetEnumerator();
                }
                if (mechDefsIterator.MoveNext())
                {
                    panel.SelectMech(mechDefsIterator.Current);
                    panel.OnEditMechClicked();
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
