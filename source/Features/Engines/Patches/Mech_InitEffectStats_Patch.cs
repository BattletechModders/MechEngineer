using BattleTech;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.InitEffectStats))]
public static class Mech_InitEffectStats_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, Mech __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        Jumping.InitEffectStats(__instance);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(Mech __instance)
    {
        EngineMisc.OverrideInitEffectStats(__instance);
    }
}
