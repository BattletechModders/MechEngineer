using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabPanel), "ValidateLoadout")]
    public static class MechLabPanel_ValidateLoadout_Patch2
    {
        public static void Postfix(MechLabPanel __instance)
        {
            try
            {
                OverrideDescriptionsHandler.Shared.RefreshData(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}