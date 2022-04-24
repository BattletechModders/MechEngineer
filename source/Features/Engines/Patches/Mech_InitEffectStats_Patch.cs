using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.InitEffectStats))]
public static class Mech_InitEffectStats_Patch
{
    public static void Prefix(Mech __instance)
    {
        try
        {
            Jumping.InitEffectStats(__instance);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }

    public static void Postfix(Mech __instance)
    {
        try
        {
            EngineMisc.OverrideInitEffectStats(__instance);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}