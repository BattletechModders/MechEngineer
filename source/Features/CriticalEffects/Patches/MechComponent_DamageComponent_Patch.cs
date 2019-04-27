using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.CriticalEffects.Patches
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

        public static void Postfix(MechComponent __instance)
        {
            try
            {
                MessagesHandler.PublishComponentState(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}