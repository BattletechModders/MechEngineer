using System;
using BattleTech;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.CalcJumpHeat))]
public static class Mech_CalcJumpHeat_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, Mech __instance, float distJumped, ref int __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            __result = Jumping.CalcJumpHeat(__instance, distJumped);
            __runOriginal = false;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
