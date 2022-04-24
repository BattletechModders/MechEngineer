using System.Collections.Generic;
using BattleTech.UI;
using DG.Tweening.Core;
using Harmony;

namespace MechEngineer.Features.Performance.Patches;

// dont really understand why DOKill is necessary, maybe just to kill fade in of an icon?
[HarmonyPatch(typeof(CombatHUDEvasiveBarPips), nameof(CombatHUDEvasiveBarPips.TrySetButton))]
public static class CombatHUDEvasiveBarPips_TrySetButton_Patch
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Method(typeof(ABSAnimationComponent), nameof(ABSAnimationComponent.DOKill)),
            AccessTools.Method(typeof(CombatHUDEvasiveBarPips_TrySetButton_Patch), nameof(DOKill))
        );
    }

    public static void DOKill(this ABSAnimationComponent @this)
    {
        // do nothing, why kill something? just let it finish animate?
    }
}