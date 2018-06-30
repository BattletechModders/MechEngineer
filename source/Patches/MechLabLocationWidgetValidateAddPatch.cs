using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabLocationWidget), "ValidateAdd", new[] {typeof(MechComponentDef)})]
    public static class MechLabLocationWidgetValidateAddPatch
    {
        public static void Postfix(
            MechLabLocationWidget __instance,
            MechComponentDef newComponentDef,
            List<MechLabItemSlotElement> ___localInventory,
            ref string ___dropErrorMessage,
            ref bool __result)
        {
            try
            {
                if (!__result)
                {
                    return;
                }

                ValidationFacade.ValidateAdd(newComponentDef, ___localInventory, ref ___dropErrorMessage, ref __result);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}