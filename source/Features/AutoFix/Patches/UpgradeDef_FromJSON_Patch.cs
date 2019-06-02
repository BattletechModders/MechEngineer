using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.AutoFix.Patches
{
    [HarmonyPatch(typeof(UpgradeDef), "FromJSON")]
    public static class UpgradeDef_FromJSON_Patch
    {
        // reduce upgrade components for the center torso that are 3 or larger 
        public static void Postfix(UpgradeDef __instance)
        {
            try
            {
                if (AutoFixerFeature.settings.UpgradeDefSkip.Contains(__instance.Description.Id))
                {
                    Control.mod.Logger.LogDebug($"AutoFixer: {__instance.Description.Id} - skipped by options");
                    return;
                }
                GyroHandler.Shared.AdjustUpgradeDef(__instance);
                LegActuatorHandler.Shared.AdjustUpgradeDef(__instance);
                CockpitHandler.Shared.AdjustUpgradeDef(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}