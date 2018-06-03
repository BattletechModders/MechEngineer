using System;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(SimGameState), "RequestDataManagerResources")]
    public static class SimGameStateRequestDataManagerResourcesPatch
    {
        // fix for https://github.com/saltyhotdog/BattletechIssueTracker/issues/10
        public static void Prefix(SimGameState __instance)
        {
            try
            {
                __instance.DataManager.RequestAllResourcesOfType(BattleTechResourceType.HeatSinkDef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}