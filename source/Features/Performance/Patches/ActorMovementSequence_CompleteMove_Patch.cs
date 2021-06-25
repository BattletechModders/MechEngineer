using BattleTech;
using Harmony;

namespace MechEngineer.Features.Performance.Patches
{
    // since we don't update on position, lets explicitly update once the movement is complete
    [HarmonyPatch(typeof(ActorMovementSequence), "CompleteMove")]
    public static class ActorMovementSequence_CompleteMove_Patch
    {
        public static void Postfix(ActorMovementSequence __instance)
        {
            AuraCache.RefreshECMStates(__instance.Combat.AllActors, EffectTriggerType.Passive);
        }
    }
}