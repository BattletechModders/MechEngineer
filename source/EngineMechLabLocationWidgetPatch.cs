using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechLabLocationWidget), "ValidateAdd", new[] { typeof(MechComponentDef) })]
    public static class EngineMechLabLocationWidgetPatch
    {
        // only allow one engine part per specific location
        public static void Postfix(
            MechLabLocationWidget __instance,
            MechComponentDef newComponentDef,
            MechLabPanel ___mechLab,
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

                if (!newComponentDef.IsEnginePart())
                {
                    return;
                }

                var existingEngine = ___localInventory
                    .Where(x => x != null)
                    .Select(x => x.ComponentRef)
                    .FirstOrDefault(x => x != null && x.Def != null && x.Def.IsEnginePart());

                if (existingEngine == null)
                {
                    return;
                }

                ___dropErrorMessage = String.Format("Cannot add {0}: An engine part is already installed", newComponentDef.Description.Name);
                __result = false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}