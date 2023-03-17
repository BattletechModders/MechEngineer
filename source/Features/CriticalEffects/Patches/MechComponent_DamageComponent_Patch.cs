using System.Collections.Generic;
using BattleTech;
using MechEngineer.Features.DamageIgnore;

namespace MechEngineer.Features.CriticalEffects.Patches;

[HarmonyPatch(typeof(MechComponent), nameof(MechComponent.DamageComponent))]
public static class MechComponent_DamageComponent_Patch
{
    [HarmonyTranspiler]
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

    [HarmonyAfter(DamageIgnoreFeature.Namespace)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechComponent __instance, WeaponHitInfo hitInfo, ref ComponentDamageLevel damageLevel)
    {
        if (!__runOriginal)
        {
            return;
        }

        var mechComponent = __instance;
        mechComponent.Criticals().Hit(hitInfo, ref damageLevel);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechComponent __instance)
    {
        MessagesHandler.PublishComponentState(__instance);
    }
}
