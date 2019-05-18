using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.TurretMechComponents.Patches
{
    [HarmonyPatch(typeof(Turret), "Init")]
    public static class Turret_Init_Patch
    {
        public static void Postfix(Turret __instance)
        {
            try
            {
                var turret = __instance;
                var num = __instance.allComponents.Select(x => int.Parse(x.uid)).DefaultIfEmpty().Max();
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
                        num++;
                        var component = new MechComponent(turret, turret.Combat, componentRef, num.ToString());
                        turret.allComponents.Add(component);
                    }
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
