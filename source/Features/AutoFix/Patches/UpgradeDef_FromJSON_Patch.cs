using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.AutoFix.Patches
{
    [HarmonyPatch(typeof(UpgradeDef), nameof(UpgradeDef.FromJSON))]
    public static class UpgradeDef_FromJSON_Patch
    {
        // reduce upgrade components for the center torso that are 3 or larger 
        public static void Postfix(UpgradeDef __instance)
        {
            try
            {
                if (AutoFixerFeature.settings.UpgradeDefSkip == null)
                {
                    return;
                }

                if (AutoFixerFeature.settings.UpgradeDefSkip.Contains(__instance.Description.Id))
                {
                    return;
                }

                if (AutoFixUtils.IsIgnoredByTags(__instance.ComponentTags, AutoFixerFeature.settings.UpgradeDefTagsSkip))
                {
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