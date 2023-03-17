using BattleTech;

namespace MechEngineer.Features.MoveMultiplierStat.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.MoveMultiplier), MethodType.Getter)]
internal static class Mech_MoveMultiplier_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(Mech __instance, ref float __result)
    {
        MoveMultiplierStatFeature.Shared.MoveMultiplier(__instance, ref __result);
    }
}
