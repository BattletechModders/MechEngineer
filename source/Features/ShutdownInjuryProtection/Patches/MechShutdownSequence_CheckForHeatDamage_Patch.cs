using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches
{
    [HarmonyPatch(typeof(MechShutdownSequence), nameof(MechShutdownSequence.CheckForHeatDamage))]
    public static class MechShutdownSequence_CheckForHeatDamage_Patch
    {
        public static bool Prepare()
        {
            return !ShutdownInjuryProtectionFeature.settings.ShutdownInjuryEnabled;
        }

        public static void Prefix(MechShutdownSequence __instance)
        {
            try
            {
                var mech = __instance.OwningMech;
                var receiveShutdownInjury = __instance.Combat.Constants.Heat.ShutdownCausesInjury
                                        || mech.StatCollection.ReceiveShutdownInjury().Get();

                if (!receiveShutdownInjury)
                {
                    return;
                }

                var sourceID = __instance.instigatorGUID;
                var stackItemUID = __instance.RootSequenceGUID;

                ShutdownInjuryProtectionFeature.InjurePilot(mech, sourceID, stackItemUID);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}