using BattleTech;
using Harmony;
using System;

namespace MechEngineer.Features.OverrideStatTooltips.Patches
{
    [HarmonyPatch(typeof(StatTooltipData), "SetMovementData")]
    public static class StatTooltipData_SetMovementData_Patch
    {
        public static void Postfix(StatTooltipData __instance, MechDef def)
        {
            try
            {
                OverrideStatTooltipsFeature.MovementStat.SetupTooltip(__instance, def);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}