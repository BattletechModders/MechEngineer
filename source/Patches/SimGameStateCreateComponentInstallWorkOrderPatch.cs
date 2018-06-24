using System;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(SimGameState), "CreateComponentInstallWorkOrder")]
    public static class SimGameStateCreateComponentInstallWorkOrderPatch
    {
        // change engine installation costs
        public static void Postfix(SimGameState __instance, MechComponentRef mechComponent, ref WorkOrderEntry_InstallComponent __result)
        {
            try
            {
                EngineMisc.ChangeInstallationCosts(mechComponent, __result);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}