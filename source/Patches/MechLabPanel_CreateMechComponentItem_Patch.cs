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
        internal static void Postfix(this MechLabPanel __instance, MechComponentRef componentRef, MechLabItemSlotElement __result)
        {
            try
            {
                EngineCoreRefHandler.Shared.ModifySlotElement(__result, __instance);
                WeightSavingsHandler.Shared.ModifySlotElement(__result, __instance);
                EnginePersistence.FixSimGameUID(__instance.sim, componentRef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}