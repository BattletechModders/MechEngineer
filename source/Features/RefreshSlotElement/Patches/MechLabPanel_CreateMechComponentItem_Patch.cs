using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using TMPro;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabPanel), "CreateMechComponentItem")]
    internal static class MechLabPanel_CreateMechComponentItem_Patch
    {
        internal static void Postfix(MechLabPanel __instance, MechComponentRef componentRef, MechLabItemSlotElement __result)
        {
            try
            {
                RefreshSlotElementHandler.Shared.RefreshSlotElement(__result, __instance);
                EnginePersistence.FixSimGameUID(__instance.sim, componentRef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}