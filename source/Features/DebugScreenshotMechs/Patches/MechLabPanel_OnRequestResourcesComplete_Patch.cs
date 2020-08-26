﻿using BattleTech.UI;
using Harmony;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace MechEngineer.Features.DebugScreenshotMechs
{
    [HarmonyPatch(typeof(MechLabPanel), "OnRequestResourcesComplete")]
    public static class MechLabPanel_OnRequestResourcesComplete_Patch
    {
        public static void Postfix(MechLabPanel __instance)
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

        private static IEnumerator CallBack(MechLabPanel panel)
        {
            yield return new WaitForEndOfFrame();
            try
            {
                var path = DebugScreenshotMechsFeature.Shared.ScreenshotPath(panel.originalMechDef);
                if (path != null)
                {
                    CaptureScreenshot(path);
                    panel.OnCancelClicked();
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
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
                byte[] bytes = tex.EncodeToPNG();

                //Save image to file
                File.WriteAllBytes(path, bytes);
            }
            finally
            {
                UnityEngine.Object.Destroy(tex);
            }
        }
    }
}
