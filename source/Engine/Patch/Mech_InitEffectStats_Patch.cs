using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "InitEffectStats")]
    public static class Mech_InitEffectStats_Patch
    {
        // change the movement stats when loading into a combat game the first time
        public static void Postfix(Mech __instance)
        {
            try
            {
                EngineMisc.InitEffectstats(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}