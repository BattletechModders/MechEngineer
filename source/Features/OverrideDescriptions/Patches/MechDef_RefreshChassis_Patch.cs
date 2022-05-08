using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideDescriptions.Patches;

[HarmonyPatch(typeof(MechDef), nameof(MechDef.RefreshChassis))]
public static class MechDef_RefreshChassis_Patch
{
    [HarmonyPostfix]
    public static void Postfix(MechDef __instance)
    {
        try
        {
            var mechDef = __instance;
            var details = mechDef.Chassis.Description.Details;

            mechDef.Description.Details = details;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}