using System;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(UpgradeDef), "FromJSON")]
    public static class UpgradeDefFromJSONPatch
    {
        // reduce upgrade components for the center torso that are 3 or larger 
        public static void Postfix(UpgradeDef __instance)
        {
            try
            {
                Gyro.AdjustGyroUpgrade(__instance);
                Cockpit.AdjustCockpitUpgrade(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}