using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.MoveMultiplierStat.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.InitEffectStats))]
public static class Mech_InitEffectStats_Patch
{
    public static void Prefix(Mech __instance)
    {
        try
        {
            MoveMultiplierStatFeature.Shared.InitEffectStats(__instance);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}