using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechComponent), "DamageComponent")]
    public static class MechComponentDamageComponentPatch
    {
        // crit engine reduces speed
        // destroyed engine destroys CT
        public static bool Prefix(MechComponent __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            try
            {
                EngineCrits.ProcessWeaponHit(__instance, hitInfo, damageLevel, applyEffects);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return true;
        }
    }
}