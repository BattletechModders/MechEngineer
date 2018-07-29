using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "CheckForHeatDamage")]
    public static class Mech_CheckForHeatDamage_Patch
    {
        public static void Prefix(Mech __instance)
        {
            try
            {
                if (!Control.settings.HeatDamageInjuryEnabled)
                {
                    return;
                }

                var mech = __instance;
                if (!mech.IsOverheated)
                {
                    return;
                }
                if (mech.StatCollection.ReceiveHeatDamageInjury())
                {
                    mech.pilot?.SetNeedsInjury(InjuryReason.NotSet);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
