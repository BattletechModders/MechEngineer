using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(ChassisDef), "FromJSON")]
    public static class ChassisDefFromJSONPatch
    {
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