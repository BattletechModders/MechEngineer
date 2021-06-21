using System;
using BattleTech;
using CustomComponents;
using Harmony;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(HardpointExtentions), nameof(HardpointExtentions.GetJJMax))]
    internal static class CC_HardpointExtentions_GetJJMax_Patch
    {
        internal static bool Prefix(MechDef mechdef, ref int __result)
        {
            try
            {
                var stats = new MechDefMovementStatistics(mechdef);
                __result = stats.JumpJetMaxCount;
                return false;
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
            return true;
        }
    }
}