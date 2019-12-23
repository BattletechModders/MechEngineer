using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(MechComponent), nameof(MechComponent.InitPassiveSelfEffects))]
    public static class MechComponent_InitPassiveSelfEffects_Patch
    {
        public static void Postfix(MechComponent __instance)
        {
            try
            {
                EngineJumpJet.InitPassiveSelfEffects(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}