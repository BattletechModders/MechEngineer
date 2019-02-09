using System;
using BattleTech;
using Harmony;
using Localize;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechComponent), "DamageComponent")]
    public static class MechComponent_DamageComponent_Patch
    {
        public static bool Prefix(MechComponent __instance, CombatGameState ___combat, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            try
            {
                if (__instance.componentDef.IsIgnoreDamage())
                {
                    return false;
                }

                if (!CirticalHitStatesHandler.Shared.ProcessWeaponHit(__instance, ___combat, hitInfo, damageLevel, applyEffects))
                {
                    return false;
                }

                if (!EngineCrits.ProcessWeaponHit(__instance, ___combat, hitInfo, damageLevel, applyEffects))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            try
            {
                if (__instance.DamageLevel == ComponentDamageLevel.Penalized)
                {
                    __instance.PublishMessage(
                        new Text("{0} CRIT", __instance.UIName),
                        FloatieMessage.MessageNature.CriticalHit
                    );
                }
                else
                {
                    __instance.PublishMessage(
                        new Text("{0} DESTROYED", __instance.UIName),
                        FloatieMessage.MessageNature.ComponentDestroyed
                    );
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return true;
        }
    }
}