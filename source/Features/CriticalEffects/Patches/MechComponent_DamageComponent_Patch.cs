using System;
using BattleTech;
using Harmony;
using Localize;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechComponent), "DamageComponent")]
    public static class MechComponent_DamageComponent_Patch
    {
        public static bool Prefix(MechComponent __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel)
        {
            try
            {
                if (__instance.componentDef.IsIgnoreDamage())
                {
                    return false;
                }

                if (!CriticalEffectsHandler.Shared.ProcessWeaponHit(__instance, hitInfo, damageLevel))
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