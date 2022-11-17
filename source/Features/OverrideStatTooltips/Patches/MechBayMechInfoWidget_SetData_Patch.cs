﻿using BattleTech.UI;
using Harmony;
using System;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(MechBayMechInfoWidget), nameof(MechBayMechInfoWidget.Awake))]
public static class MechBayMechInfoWidget_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix(LanceStat[] ___mechStats)
    {
        try
        {
            SetMechStats(___mechStats);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }

    internal static void SetMechStats(LanceStat[] mechStats)
    {
        var settings = OverrideStatTooltipsFeature.Shared.Settings;
        mechStats[0].SetText(settings.FirepowerTitleText);
        mechStats[1].SetText(settings.MovementTitleText);
        mechStats[2].SetText(settings.DurabilityTitleText);
        mechStats[3].SetText(settings.HeatEfficiencyTitleText);
        mechStats[4].SetText(settings.AvgRangeTitleText);
        mechStats[5].SetText(settings.MeleeTitleText);
    }
}
