using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(SkirmishSettings_Beta), "LoadLanceConfiguratorData")]
    public static class SkirmishSettings_BetaLoadLanceConfiguratorDataPatch
    {
        public static void Prefix(SkirmishSettings_Beta __instance, UIManager ___uiManager)
        {
            try
            {
                ___uiManager.dataManager.RequestAllResourcesOfType(BattleTechResourceType.HeatSinkDef);
                ___uiManager.dataManager.RequestAllResourcesOfType(BattleTechResourceType.UpgradeDef);
                ___uiManager.dataManager.RequestAllResourcesOfType(BattleTechResourceType.WeaponDef);
                ___uiManager.dataManager.RequestAllResourcesOfType(BattleTechResourceType.AmmunitionBoxDef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}