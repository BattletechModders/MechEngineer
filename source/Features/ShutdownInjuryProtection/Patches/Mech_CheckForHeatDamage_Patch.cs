using System;
using BattleTech;
using Harmony;
using MechEngineer.Misc;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.CheckForHeatDamage))]
public static class Mech_CheckForHeatDamage_Patch
{
    [UsedByHarmony]
    public static bool Prepare()
    {
        return ShutdownInjuryProtectionFeature.settings.HeatDamageInjuryEnabled;
    }

    [HarmonyPrefix]
    public static void Prefix(Mech __instance, int stackID, string attackerID)
    {
        try
        {
            var mech = __instance;
            if (!mech.StatCollection.ReceiveHeatDamageInjury().Get())
            {
                return;
            }

            ShutdownInjuryProtectionFeature.SetInjury(mech, attackerID, stackID);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}