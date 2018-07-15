using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(SimGameState), "RequestDataManagerResources")]
    public static class SimGameState_RequestDataManagerResources_Patch
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