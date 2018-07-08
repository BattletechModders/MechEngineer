using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
    public static class MechLabPanel_LoadMech_Patch
    {
        public static void Postfix(MechLabPanel __instance)
        {
            try
            {
                DynamicSlotHandler.Shared.RefreshData(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}