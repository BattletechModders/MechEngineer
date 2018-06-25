using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabInventoryWidget), "RefreshJumpJetOptions")]
    public static class MechLabInventoryWidgetRefreshJumpJetOptionsPatch
    {
        // hide incompatible engines
        public static void Postfix(MechLabInventoryWidget __instance, float ___mechTonnage)
        {
            try
            {
                if (!Control.settings.EnableAvailabilityChecks)
                {
                    return;
                }
                EngineMisc.RefreshAvailability(__instance, ___mechTonnage);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}