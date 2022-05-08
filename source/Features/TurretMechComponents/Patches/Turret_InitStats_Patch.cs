using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.TurretMechComponents.Patches;

[HarmonyPatch(typeof(Turret), nameof(Turret.InitStats))]
public static class Turret_InitStats_Patch
{
    [HarmonyPrefix]
    public static void Prefix(Turret __instance)
    {
        try
        {
            if (__instance.Combat.IsLoadingFromSave)
            {
                return;
            }

            var turret = __instance;

            var num = 0;

            foreach (var componentRef in turret.TurretDef.Inventory)
            {
                if (componentRef.ComponentDefType == ComponentType.Weapon)
                {
                }
                else if (componentRef.ComponentDefType == ComponentType.AmmunitionBox)
                {
                }
                else
                {
                    componentRef.DataManager = turret.TurretDef.DataManager;
                    componentRef.RefreshComponentDef();

                    var uid = $"{num++}_addedByME";
                    var component = new MechComponent(turret, turret.Combat, componentRef, uid);
                    turret.allComponents.Add(component);
                }
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}