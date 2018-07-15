using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(SimGameState), "RespondToDefsLoadComplete")]
    public static class SimGameState_RespondToDefsLoadComplete_Patch
    {
        public static void Prefix(SimGameState __instance)
        {
            try
            {
                MechDefAutoFixFacade.PostProcessAfterLoading(__instance.DataManager);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}