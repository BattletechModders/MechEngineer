using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(SkirmishSettings_Beta), "LanceConfiguratorDataLoaded")]
    public static class SkirmishSettings_BetaLanceConfiguratorDataLoadedPatch
    {
        public static void Prefix(SkirmishSettings_Beta __instance, UIManager ___uiManager)
        {
            try
            {
                MechDefAutoFixFacade.PostProcessAfterLoading(___uiManager.dataManager);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}