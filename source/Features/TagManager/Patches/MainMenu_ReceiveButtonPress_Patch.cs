using System;
using BattleTech.UI;
using Harmony;
using MechEngineer.Misc;

namespace MechEngineer.Features.TagManager.Patches;

[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.ReceiveButtonPress))]
public static class MainMenu_ReceiveButtonPress_Patch
{
    [UsedByHarmony]
    public static bool Prepare()
    {
        return TagManagerFeature.Shared.Settings.SkirmishOptionsShow;
    }

    [HarmonyPrefix]
    public static bool Prefix(MainMenu __instance, string button)
    {
        try
        {
            if (button == "MechBay")
            {
                TagManagerFeature.Shared.ShowOptions(__instance);
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
            return false;
        }
        return true;
    }
}