using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.MoveMultiplierStat.Patches
{
    [HarmonyPatch(typeof(Mech), "MoveMultiplier", MethodType.Getter)]
    internal static class Mech_MoveMultiplier_Patch
    {
        public static void Postfix(Mech __instance, ref float __result)
        {
            try
            {
                MoveMultiplierStatHandler.MoveMultiplier(__instance, ref __result);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}