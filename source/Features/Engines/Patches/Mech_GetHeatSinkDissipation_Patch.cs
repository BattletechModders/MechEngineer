using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(Mech), "GetHeatSinkDissipation")]
    public static class Mech_GetHeatSinkDissipation_Patch
    {
        // get heat dissipation rate of the engine by inventory and rating
        public static void Postfix(Mech __instance, ref float __result)
        {
            try
            {
                __result += EngineHeat.GetEngineHeatDissipation(__instance.MechDef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}