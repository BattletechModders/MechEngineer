using System;
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
        public static void Postfix(MechLabLocationWidget __instance, MechComponentDef newComponentDef, ref bool __result)
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

                var adapter = new MechLabLocationWidgetAdapter(__instance);
                if (adapter.LocalInventory.Select(x => x.ComponentRef).All(x => x == null || !x.Def.IsEnginePart()))
                {
                    return;
                }

                adapter.DropErrorMessage = String.Format("Cannot add {0}: An engine part is already installed", newComponentDef.Description.Name);
                __result = false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}