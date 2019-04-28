using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.BattleTechLoadFix.Patches
{
    [HarmonyPatch(typeof(SkirmishMechBayPanel), "RequestResources")]
    public static class SkirmishMechBayPanel_RequestResources_Patch
    {
        public static void Prefix(SkirmishMechBayPanel __instance)
        {
            try
            {
                BattleTechLoadFixFeature.PreloadComponents(__instance.dataManager);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}