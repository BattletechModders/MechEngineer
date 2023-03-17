using BattleTech;

namespace MechEngineer.Features.Performance.Patches;

// disable stealth preview, takes alot of resources
[HarmonyPatch(typeof(AbstractActor), nameof(AbstractActor.StealthPipsPreviewFromActorMovement))]
public static class AbstractActor_StealthPipsPreviewFromActorMovement_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, AbstractActor __instance, ref int __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        __result = 0;
        __runOriginal = false;
    }
}
