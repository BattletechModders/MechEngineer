using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(ChassisDef), "FromJSON")]
    public static class InitialTonnageChassisDefPatch
    {
        // set initial weight of mechs to 0.1 times the tonnage
        public static void Postfix(ChassisDef __instance)
        {
            try
            {
                if (!Control.settings.InitialTonnageOverride)
                {
                    return;
                }

                if (Control.settings.InitialTonnageOverrideSkipChassis.Contains(__instance.Description.Id))
                {
                    return;
                }

                var value = __instance.Tonnage * Control.settings.InitialToTotalTonnageFactor;
                var propInfo = typeof(ChassisDef).GetProperty("InitialTonnage");
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