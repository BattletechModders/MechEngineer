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
    [HarmonyPostfix]
    public static void Postfix(SkirmishMechBayPanel __instance)
    {
        try
        {
            __instance.StartCoroutine(CallBack(__instance));
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }

    private static IEnumerator<MechDef>? mechDefsIterator;
    private static IEnumerator CallBack(SkirmishMechBayPanel panel)
    {
        yield return new WaitForEndOfFrame();
        try
        {
            if (mechDefsIterator == null)
            {
                mechDefsIterator = panel.allMechs.OrderBy(x => x.Description.Id).GetEnumerator();
            }
            while (mechDefsIterator.MoveNext())
            {
                var mechDef = mechDefsIterator.Current;
                if (mechDef == null)
                {
                    continue;
                }
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
            Log.Main.Error?.Log(e);
        }
    }
}
