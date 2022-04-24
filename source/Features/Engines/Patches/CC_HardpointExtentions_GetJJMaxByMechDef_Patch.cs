using System;
using BattleTech;
using CustomComponents;
using Harmony;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.Engines.Patches;

[HarmonyPatch(typeof(HardpointExtentions), nameof(HardpointExtentions.GetJJMaxByMechDef))]
internal static class CC_HardpointExtentions_GetJJMaxByMechDef_Patch
{
    internal static bool Prefix(MechDef def, ref int __result)
    {
        try
        {
            if (def == null)
            {
                __result = -1;
            }
            else
            {
                var stats = new MechDefMovementStatistics(def);
                __result = stats.JumpJetMaxCount;
            }
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}