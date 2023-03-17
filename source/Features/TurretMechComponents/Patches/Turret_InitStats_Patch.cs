using BattleTech;

namespace MechEngineer.Features.TurretMechComponents.Patches;

[HarmonyPatch(typeof(Turret), nameof(Turret.InitStats))]
public static class Turret_InitStats_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, Turret __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

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
}
