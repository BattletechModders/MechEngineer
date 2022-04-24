using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.OnActivationEnd))]
public static class Mech_OnActivationEnd_Patch
{
    public static bool Prepare()
    {
        return !ShutdownInjuryProtectionFeature.settings.OverheatedOnActivationEndInjuryEnabled;
    }

    public static void Prefix(Mech __instance, string sourceID, int stackItemID)
    {
        try
        {
            var mech = __instance;
            if (mech.HasActivatedThisRound)
            {
                return;
            }
            if (!mech.StatCollection.ReceiveOverheatedOnActivationEndInjury().Get())
            {
                return;
            }
            ShutdownInjuryProtectionFeature.SetInjury(mech, sourceID, stackItemID);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}