using System;
using BattleTech;
using Harmony;
using MechEngineer.Misc;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.OnActivationEnd))]
public static class Mech_OnActivationEnd_Patch
{
    [UsedByHarmony]
    public static bool Prepare()
    {
        return ShutdownInjuryProtectionFeature.settings.OverheatedOnActivationEndInjuryEnabled;
    }

    [HarmonyPrefix]
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
            Log.Main.Error?.Log(e);
        }
    }
}
