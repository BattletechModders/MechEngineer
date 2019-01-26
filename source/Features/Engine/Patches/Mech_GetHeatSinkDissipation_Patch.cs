using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "GetHeatSinkDissipation")]
    public static class Mech_GetHeatSinkDissipation_Patch
    {
        // get heat dissipation rate of the engine by inventory and rating
        public static void Postfix(Mech __instance, ref float __result)
        {
            try
            {
                __result += EngineHeat.GetEngineHeatDissipation(__instance.MechDef.Inventory);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}