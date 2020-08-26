using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.DamageIgnore.Patches
{
    [HarmonyPatch(typeof(MechComponent), nameof(MechComponent.DamageComponent))]
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
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }

            return true;
        }
    }
}