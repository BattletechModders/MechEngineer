using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.JumpDistance), MethodType.Getter)]
public static class Mech_JumpDistance_Patch
{
    public static bool Prefix(Mech __instance, ref float __result)
    {
        try
        {
            __result = Jumping.CalcMaxJumpDistance(__instance);
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}