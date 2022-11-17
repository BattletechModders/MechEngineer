using System;
using BattleTech;
using Harmony;
using MechEngineer.Misc;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches;

[HarmonyPatch(typeof(MechShutdownSequence), nameof(MechShutdownSequence.CheckForHeatDamage))]
public static class MechShutdownSequence_CheckForHeatDamage_Patch
{
    [UsedByHarmony]
    public static bool Prepare()
    {
        return ShutdownInjuryProtectionFeature.settings.ShutdownInjuryEnabled;
    }

    [HarmonyPrefix]
    public static bool Prefix(MechShutdownSequence __instance)
    {
        try
        {
            var mech = __instance.OwningMech;
            var receiveShutdownInjury = __instance.Combat.Constants.Heat.ShutdownCausesInjury
                                        || mech.StatCollection.ReceiveShutdownInjury().Get();

            if (receiveShutdownInjury && mech.IsOverheated)
            {
                var sourceID = __instance.instigatorGUID;
                var stackItemUID = __instance.RootSequenceGUID;

                ShutdownInjuryProtectionFeature.SetInjury(mech, sourceID, stackItemUID);
            }

            return false;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }

        return true;
    }
}
