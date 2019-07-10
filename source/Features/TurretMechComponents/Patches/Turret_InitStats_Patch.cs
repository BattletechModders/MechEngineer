using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.TurretMechComponents.Patches
{
    [HarmonyPatch(typeof(Turret), "InitStats")]
    public static class Turret_InitStats_Patch
    {
        public static void Prefix(Turret __instance)
        {
            try
            {
                if (__instance.Combat.IsLoadingFromSave)
                {
                    return;
                }

                var turret = __instance;
                
                var num = __instance.allComponents
                    .Where(x => x != null)
                    .Select(x => int.TryParse(x.uid, out var result) ? result : (int?)null)
                    .Where(x => x != null)
                    .DefaultIfEmpty()
                    .Max();

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
