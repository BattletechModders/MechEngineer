using System;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "InitEffectStats")]
    public static class Mech_InitEffectStats_Patch5
    {
        public static void Prefix(Mech __instance)
        {
            try
            {
                AccuracyEffects.SetupAccuracyStatistics(__instance.StatCollection);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}