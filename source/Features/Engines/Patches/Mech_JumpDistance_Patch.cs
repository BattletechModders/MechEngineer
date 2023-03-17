using BattleTech;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.JumpDistance), MethodType.Getter)]
public static class Mech_JumpDistance_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, Mech __instance, ref float __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        __result = Jumping.CalcMaxJumpDistance(__instance);
        __runOriginal = false;
    }
}
