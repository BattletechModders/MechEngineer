using System;
using BattleTech;
using Harmony;
using Localize;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechComponent), "DamageComponent")]
    public static class MechComponent_DamageComponent_Patch
    {
        public static bool Prefix(MechComponent __instance, WeaponHitInfo hitInfo, ref ComponentDamageLevel damageLevel)
        {
            try
            {
                var mechComponent = __instance;
                if (mechComponent.componentDef.IsIgnoreDamage())
                {
                    return false;
                }

                CriticalEffectsHandler.Shared.ProcessWeaponHit(mechComponent, hitInfo, ref damageLevel);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return true;
        }

        public static void Postfix(MechComponent __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel)
        {
            try
            {
                var mechComponent = __instance;
                if (mechComponent.DamageLevel == ComponentDamageLevel.Penalized)
                {
                    mechComponent.PublishMessage(
                        new Text("{0} CRIT", __instance.UIName),
                        FloatieMessage.MessageNature.CriticalHit
                    );
                }
                else if (mechComponent.DamageLevel == ComponentDamageLevel.Destroyed)
                {
                    // dont show destroyed if a mech is known to be incapacitated
                    var actor = mechComponent.parent;
                    if (actor.IsDead || actor.IsFlaggedForDeath)
                    {
                        return;
                    }

                    // dont show destroyed if a whole section was destroyed, counters spam
                    //if (actor is Mech mech)
                    //{
                    //    var location = mechComponent.mechComponentRef.MountedLocation;
                    //    var mechLocationDestroyed = mech.IsLocationDestroyed(location);
                    //    if (mechLocationDestroyed)
                    //    {
                    //        return;
                    //    }
                    //}

                    mechComponent.PublishMessage(
                        new Text("{0} DESTROYED", __instance.UIName),
                        FloatieMessage.MessageNature.ComponentDestroyed
                    );
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}