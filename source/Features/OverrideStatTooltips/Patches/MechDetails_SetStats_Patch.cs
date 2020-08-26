﻿using System;
using System.Collections.Generic;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.OverrideStatTooltips.Patches
{
    [HarmonyPatch(typeof(MechDetails), "SetStats")]
    public static class MechDetails_SetStats_Patch
    {
        public static void Postfix(List<LanceStat> ___statList)
        {
            try
            {
				var settings = OverrideStatTooltipsFeature.Shared.Settings;
			    ___statList[1].SetText(settings.HeatEfficiencyTitleText);
			    ___statList[2].SetText(settings.MovementTitleText);
			    ___statList[3].SetText(settings.AvgRangeTitleText);
			    ___statList[4].SetText(settings.DurabilityTitleText);
			    ___statList[5].SetText(settings.FirepowerTitleText);
			    ___statList[7].SetText(settings.MeleeTitleText);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}