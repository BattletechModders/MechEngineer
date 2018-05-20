using System;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(UpgradeDef), "FromJSON")]
    public static class GyroUpgradeDefPatch
    {
        // reduce upgrade components for the center torso that are 3 or larger 
        public static void Postfix(UpgradeDef __instance)
        {
            try
            {
                if (!__instance.IsCenterTorsoUpgrade())
                {
                    return;
                }

                if (__instance.InventorySize < 3)
                {
                    return;
                }

                var value = __instance.InventorySize - 2;
                var propInfo = typeof(UpgradeDef).GetProperty("InventorySize");
                var propValue = Convert.ChangeType(value, propInfo.PropertyType);
                propInfo.SetValue(__instance, propValue, null);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}