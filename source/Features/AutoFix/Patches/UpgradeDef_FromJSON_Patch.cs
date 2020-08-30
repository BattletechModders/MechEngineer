﻿using System;
using BattleTech;
using CustomComponents;
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
                var def = __instance;

                if (def.ComponentTags.IgnoreAutofix())
                {
                    return;
                }

                GyroHandler.Shared.AdjustUpgradeDef(def);
                LegActuatorHandler.Shared.AdjustUpgradeDef(def);
                CockpitHandler.Shared.AdjustUpgradeDef(def);
                SensorsBHandler.Shared.AdjustUpgradeDef(def);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}