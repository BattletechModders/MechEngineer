using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.BattleTechLoadFix.Patches
{
    [HarmonyPatch(typeof(SkirmishSettings_Beta), "LoadLanceConfiguratorData")]
    public static class SkirmishSettings_Beta_LoadLanceConfiguratorData_Patch
    {
        public static void Prefix(SkirmishSettings_Beta __instance, UIManager ___uiManager)
        {
            try
            {
                BattleTechLoadFixFeature.PreloadComponents(___uiManager.dataManager);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}