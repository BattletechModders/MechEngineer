using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(Mech), "GetHeatSinkDissipation")]
    public static class MechGetHeatSinkDissipationPatch
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