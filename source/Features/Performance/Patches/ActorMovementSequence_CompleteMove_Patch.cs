using BattleTech;
using Harmony;

namespace MechEngineer.Features.Performance.Patches;

// since we don't update on position, lets explicitly update once the movement is complete
[HarmonyPatch(typeof(ActorMovementSequence), nameof(ActorMovementSequence.CompleteMove))]
public static class ActorMovementSequence_CompleteMove_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ActorMovementSequence __instance)
    {
        AuraCache.RefreshECMStates(__instance.Combat.AllActors, EffectTriggerType.Passive);
    }
}