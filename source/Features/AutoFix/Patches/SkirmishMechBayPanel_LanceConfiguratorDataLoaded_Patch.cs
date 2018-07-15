using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(SkirmishMechBayPanel), "LanceConfiguratorDataLoaded")]
    public static class SkirmishMechBayPanel_LanceConfiguratorDataLoaded_Patch
    {
        public static void Prefix(SkirmishMechBayPanel __instance)
        {
            try
            {
                MechDefAutoFixFacade.PostProcessAfterLoading(__instance.dataManager);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}