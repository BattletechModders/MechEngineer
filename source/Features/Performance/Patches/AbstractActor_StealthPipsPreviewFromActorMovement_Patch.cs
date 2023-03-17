using System;
using BattleTech;

namespace MechEngineer.Features.Performance.Patches;

// disable stealth preview, takes alot of resources
[HarmonyPatch(typeof(AbstractActor), nameof(AbstractActor.StealthPipsPreviewFromActorMovement))]
public static class AbstractActor_StealthPipsPreviewFromActorMovement_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, AbstractActor __instance, ref int __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            __result = 0;
            __runOriginal = false;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
