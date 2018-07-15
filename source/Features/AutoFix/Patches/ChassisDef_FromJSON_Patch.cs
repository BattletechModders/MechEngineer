using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(ChassisDef), "FromJSON")]
    public static class ChassisDef_FromJSON_Patch
    {
        public static void Postfix(ChassisDef __instance)
        {
            try
            {
                ChassisHandler.OverrideChassisSettings(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
