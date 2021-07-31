using BattleTech;
using Harmony;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches
{
    [HarmonyPatch(typeof(Mech), nameof(Mech.OnActivationEnd))]
    public static class Mech_OnActivationEnd_Patch
    {
        public static bool Prepare()
        {
            return !ShutdownInjuryProtectionFeature.settings.OverheatedOnActivationEndInjuryEnabled;
        }

        public static void Prefix(Mech __instance, string sourceID, int stackItemID)
        {
            var mech = __instance;
            if (mech.HasActivatedThisRound)
            {
                return;
            }
            if (!mech.IsOverheated)
            {
                return;
            }
            if (!mech.StatCollection.ReceiveOverheatedOnActivationEndInjury().Get())
            {
                return;
            }
            var pilot = mech.pilot;
            if (pilot == null)
            {
                return;
            }
            pilot.SetNeedsInjury(Pilot_InjuryReasonDescription_Patch.InjuryReasonOverheated);
            // TODO check why these two steps below are necessary (is this new to directly apply injuries to allow more than one per activation?)
            // TODO shouldn't a mod just do these steps as part of SetNeedsInjury?
            pilot.InjurePilot(sourceID, stackItemID, 1, DamageType.Overheat, default, mech);
            pilot.ClearNeedsInjury();
        }
    }
}
