using BattleTech.UI;
using Harmony;
using System;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(MechLabStatBlockWidget), nameof(MechLabStatBlockWidget.Awake))]
public static class MechLabStatBlockWidget_Awake_Patch
{
    public static void Postfix(LanceStat[] ___mechStats)
    {
        try
        {
            MechBayMechInfoWidget_Awake_Patch.SetMechStats(___mechStats);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}