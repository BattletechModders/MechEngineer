using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabPanel), "CreateMechComponentItem")]
    internal static class MechLabPanel_CreateMechComponentItem_Patch2
    {
        internal static void Postfix(MechLabPanel __instance, MechComponentRef componentRef, MechLabItemSlotElement __result)
        {
            try
            {
                OverrideDescriptionsHandler.Shared.AdjustSlotElement(__result, __instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}