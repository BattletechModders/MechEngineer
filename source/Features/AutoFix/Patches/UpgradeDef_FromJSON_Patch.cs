using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(UpgradeDef), "FromJSON")]
    public static class UpgradeDef_FromJSON_Patch
    {
        // reduce upgrade components for the center torso that are 3 or larger 
        public static void Postfix(UpgradeDef __instance)
        {
            try
            {
                GyroHandler.Shared.AdjustUpgradeDef(__instance);
                LegUpgradeHandler.Shared.AdjustUpgradeDef(__instance);
                CockpitHandler.Shared.AdjustUpgradeDef(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}