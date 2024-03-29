﻿using BattleTech;
using MechEngineer.Helper;

namespace MechEngineer.Features.AutoFix.Patches;

[HarmonyPatch(typeof(UpgradeDef), nameof(UpgradeDef.FromJSON))]
public static class UpgradeDef_FromJSON_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    // reduce upgrade components for the center torso that are 3 or larger 
    public static void Postfix(UpgradeDef __instance)
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
}
