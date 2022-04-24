using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace MechEngineer.Features.DebugScreenshotMechs.Patches;

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
            Control.Logger.Error.Log(e);
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
            while (mechDefsIterator.MoveNext())
            {
                var mechDef = mechDefsIterator.Current;
                var screenshotPath = DebugScreenshotMechsFeature.Shared.ScreenshotPath(mechDef);
                if (File.Exists(screenshotPath))
                {
                    continue;
                }
                if (DebugScreenshotMechsFeature.Shared.Settings.OnlyInvalidMechs
                    && MechValidationRules.ValidateMechDef(MechValidationLevel.Full, panel.dataManager, mechDef, null).All(x => !x.Value.Any()))
                {
                    continue;
                }
                panel.SelectMech(mechDef);
                panel.OnEditMechClicked();
                break;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}