using BattleTech.UI;
using Harmony;
using System;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(MechLabStatBlockWidget), nameof(MechLabStatBlockWidget.Awake))]
public static class MechLabStatBlockWidget_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix(LanceStat[] ___mechStats)
    {
        try
        {
            MechBayMechInfoWidget_Awake_Patch.SetMechStats(___mechStats);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
