using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechLabLocationWidget), "ValidateAdd", new[] { typeof(MechComponentDef) })]
    public static class GyroMechLabLocationWidgetPatch
    {
        // allow gyro upgrades to be 1 slot and still only be added once
        public static void Postfix(MechLabLocationWidget __instance, MechComponentDef newComponentDef, ref bool __result)
        {
            try
            {
                if (!__result)
                {
                    return;
                }

                if (!Control.IsCenterTorsoUpgrade(newComponentDef))
                {
                    return;
                }

                var adapter = new MechLabLocationWidgetAdapter(__instance);
                if (adapter.LocalInventory.Select(x => x.ComponentRef).All(x => x == null || !Control.IsCenterTorsoUpgrade(x.Def)))
                {
                    return;
                }

                adapter.DropErrorMessage = String.Format("Cannot add {0}: A center torso upgrade is already installed", newComponentDef.Description.Name);
                __result = false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}