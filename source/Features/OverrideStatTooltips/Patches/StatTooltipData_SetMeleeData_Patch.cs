﻿using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideStatTooltips.Patches
{
    [HarmonyPatch(typeof(StatTooltipData), "SetMeleeData")]
    public static class StatTooltipData_SetMeleeData_Patch
    {
        public static void Postfix(StatTooltipData __instance, MechDef def)
        {
            try
            {
                OverrideStatTooltipsFeature.MeleeStat.SetupTooltip(__instance, def);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}