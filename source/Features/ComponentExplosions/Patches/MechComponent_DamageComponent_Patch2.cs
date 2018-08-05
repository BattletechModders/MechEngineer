using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechComponent), nameof(MechComponent.DamageComponent))]
    public static class MechComponent_DamageComponent_Patch2
    {
        public static void Postfix(MechComponent __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            try
            {
                ComponentExplosionHandler.Shared.CheckForExplosion(__instance, hitInfo, damageLevel, applyEffects);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}