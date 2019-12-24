using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(Mech), "InitEffectStats")]
    public static class Mech_InitEffectStats_Patch
    {
        public static void Prefix(Mech __instance)
        {
            try
            {
                EngineJumpJet.InitEffectStats(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        public static void Postfix(Mech __instance)
        {
            try
            {
                EngineMisc.OverrideInitEffectStats(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}