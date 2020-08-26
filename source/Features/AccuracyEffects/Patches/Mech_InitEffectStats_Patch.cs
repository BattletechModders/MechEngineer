﻿using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.AccuracyEffects.Patches
{
    [HarmonyPatch(typeof(Mech), "InitEffectStats")]
    public static class Mech_InitEffectStats_Patch
    {
        public static void Prefix(Mech __instance)
        {
            try
            {
                AccuracyEffectsFeature.SetupAccuracyStatistics(__instance.StatCollection);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}