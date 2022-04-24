using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.HeatSinkCapacityStat.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.InitEffectStats))]
public static class Mech_InitEffectStats_Patch
{
    public static void Postfix(Mech __instance)
    {
        try
        {
            HeatSinkCapacityStatFeature.Shared.InitEffectStats(__instance);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}