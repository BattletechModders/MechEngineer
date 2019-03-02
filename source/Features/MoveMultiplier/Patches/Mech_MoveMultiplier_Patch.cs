using System;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "MoveMultiplier")]
    internal static class Mech_MoveMultiplier_Patch
    {
        public static void Postfix(Mech __instance, ref float __result)
        {
            try
            {
                var multiplier = __instance.StatCollection.MoveMultiplier();
                var rounded = Mathf.Max(__instance.Combat.Constants.MoveConstants.MinMoveSpeed, multiplier);
                __result = __result * rounded;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}