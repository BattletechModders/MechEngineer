using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.MoveMultiplierStat.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.MoveMultiplier), MethodType.Getter)]
internal static class Mech_MoveMultiplier_Patch
{
    [HarmonyPostfix]
    public static void Postfix(Mech __instance, ref float __result)
    {
        try
        {
            MoveMultiplierStatFeature.Shared.MoveMultiplier(__instance, ref __result);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}