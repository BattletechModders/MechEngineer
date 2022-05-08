using System.Collections.Generic;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer.Features.Performance.Patches;

// dont recalculate stealth aura on movement
[HarmonyPatch(typeof(AbstractActor), nameof(AbstractActor.OnPositionUpdate))]
public static class AbstractActor_OnPositionUpdate_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Method(typeof(AuraCache), nameof(AuraCache.UpdateAurasToActor)),
            AccessTools.Method(typeof(AbstractActor_OnPositionUpdate_Patch), nameof(UpdateAurasToActor))
        );
    }

    public static void UpdateAurasToActor(
        List<AbstractActor> actors,
        AbstractActor movingActor,
        Vector3 movingActorPosition,
        EffectTriggerType triggerSource,
        bool forceUpdate)
    {
        // ignore position updates
    }
}