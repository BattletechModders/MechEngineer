﻿using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.TagManager.Patches;

[HarmonyPatch(typeof(SkirmishMechBayPanel), nameof(SkirmishMechBayPanel.RequestResources))]
public static class SkirmishMechBayPanel_RequestResources_Patch
{
    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    public static bool Prefix(SkirmishMechBayPanel __instance)
    {
        try
        {
            TagManagerFeature.Shared.RequestResources(__instance);
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}