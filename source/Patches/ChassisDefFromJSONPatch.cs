using System;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(ChassisDef), "FromJSON")]
    public static class ChassisDefFromJSONPatch
    {
        // set initial weight of mechs to 0.1 times the tonnage
        public static void Postfix(ChassisDef __instance)
        {
            try
            {
                Chassis.OverrideChassisSettings(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}