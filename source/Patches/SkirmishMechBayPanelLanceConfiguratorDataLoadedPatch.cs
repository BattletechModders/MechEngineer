using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(SkirmishMechBayPanel), "LanceConfiguratorDataLoaded")]
    public static class SkirmishMechBayPanelLanceConfiguratorDataLoadedPatch
    {
        public static void Prefix(SkirmishMechBayPanel __instance)
        {
            try
            {
                MechDefMods.PostProcessAfterLoading(__instance.dataManager);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}