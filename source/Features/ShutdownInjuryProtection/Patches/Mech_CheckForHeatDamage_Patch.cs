using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches
{
    [HarmonyPatch(typeof(Mech), nameof(Mech.CheckForHeatDamage))]
    public static class Mech_CheckForHeatDamage_Patch
    {
        public static bool Prepare()
        {
            return !ShutdownInjuryProtectionFeature.settings.HeatDamageInjuryEnabled;
        }

        public static void Prefix(Mech __instance)
        {
            try
            {
                var mech = __instance;
                if (!mech.IsOverheated)
                {
                    return;
                }
                if (mech.StatCollection.ReceiveHeatDamageInjury().Get())
                {
                    mech.pilot?.SetNeedsInjury(Pilot_InjuryReasonDescription_Patch.InjuryReasonOverheated);
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}
