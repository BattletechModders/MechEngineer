using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.CriticalEffects.Patches
{
    [HarmonyPatch(typeof(MechComponent), nameof(MechComponent.DamageComponent))]
    public static class MechComponent_DamageComponent_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(MechComponent), nameof(MechComponent.CancelCreatedEffects)),
                AccessTools.Method(typeof(MechComponent_DamageComponent_Patch), nameof(CancelCreatedEffects))
            );
        }

        public static void CancelCreatedEffects(MechComponent mechComponent, bool performAuraRefresh = true)
        {
            if (mechComponent.DamageLevel >= ComponentDamageLevel.NonFunctional)
            {
                mechComponent.CancelCreatedEffects(performAuraRefresh);
            }
        }

        public static bool Prefix(MechComponent __instance, WeaponHitInfo hitInfo, ref ComponentDamageLevel damageLevel)
        {
            try
            {
                var mechComponent = __instance;
                if (mechComponent.componentDef.IsIgnoreDamage())
                {
                    return false;
                }

                CriticalEffectsFeature.Shared.ProcessWeaponHit(mechComponent, hitInfo, ref damageLevel);
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