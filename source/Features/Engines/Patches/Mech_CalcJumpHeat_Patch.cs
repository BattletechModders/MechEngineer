using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(Mech), nameof(Mech.CalcJumpHeat))]
    public static class Mech_CalcJumpHeat_Patch
    {
        public static bool Prefix(Mech __instance, float distJumped, ref int  __result)
        {
            try
            {
                __result = Jumping.CalcJumpHeat(__instance, distJumped);
                return false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return true;
        }
    }
}