using BattleTech;
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
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, Mech __instance, int stackID, string attackerID)
    {
        if (!__runOriginal)
        {
            return;
        }

        var mech = __instance;
        if (!mech.StatCollection.ReceiveHeatDamageInjury().Get())
        {
            return;
        }

        ShutdownInjuryProtectionFeature.SetInjury(mech, attackerID, stackID);
    }
}
