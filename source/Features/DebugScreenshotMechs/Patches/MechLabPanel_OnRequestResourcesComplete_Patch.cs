﻿using System;
using System.Collections;
using System.IO;
using BattleTech.UI;
using UnityEngine;

namespace MechEngineer.Features.DebugScreenshotMechs.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.OnRequestResourcesComplete))]
public static class MechLabPanel_OnRequestResourcesComplete_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechLabPanel __instance)
    {
        __instance.StartCoroutine(CallBack(__instance));
    }

    private static IEnumerator CallBack(MechLabPanel panel)
    {
        yield return new WaitForEndOfFrame();
        try
        {
            if (DebugScreenshotMechsFeature.Shared.Settings.CaptureScreenshots)
            {
                var path = DebugScreenshotMechsFeature.Shared.ScreenshotPath(panel.originalMechDef);
                if (path != null)
                {
                    CaptureScreenshot(path);
                }
            }
            if (DebugScreenshotMechsFeature.Shared.Settings.DumpGameObjectCounts)
            {
                SceneMemoryStatsDumper.DumpUiStuff();
            }
            panel.OnCancelClicked();
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }

    private static void CaptureScreenshot(string path)
    {
        // Create a texture the size of the screen, RGB24 format
        var width = Screen.width;
        var height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        try
        {
            // Read screen contents into the texture
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            // Encode texture into PNG
            var bytes = tex.EncodeToPNG();

            //Save image to file
            File.WriteAllBytes(path, bytes);
        }
        finally
        {
            UnityEngine.Object.Destroy(tex);
        }
    }
}
