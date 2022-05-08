using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.HeatSinkCapacityStat.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.RestartCreatedEffects))]
public static class Mech_RestartCreatedEffects_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions
            .MethodReplacer(
                AccessTools.Method(typeof(MechComponent), nameof(MechComponent.RestartPassiveEffects)),
                AccessTools.Method(typeof(Mech_RestartCreatedEffects_Patch), nameof(RestartPassiveEffects))
            );
    }

    public static void RestartPassiveEffects(this MechComponent @this, bool performAuraRefresh)
    {
        if (HeatSinkCapacityStatFeature.Shared.IgnoreShutdown(@this))
        {
            return;
        }
        @this.RestartPassiveEffects(performAuraRefresh);
    }
}