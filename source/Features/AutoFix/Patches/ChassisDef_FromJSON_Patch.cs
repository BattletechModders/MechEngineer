using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.AutoFix.Patches;

[HarmonyPatch(typeof(ChassisDef), nameof(ChassisDef.FromJSON))]
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
            Control.Logger.Error.Log(e);
        }
    }
}