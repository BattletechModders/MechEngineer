using System;
using BattleTech;
using Harmony;
using Localize;
using MechEngineer.Misc;

namespace MechEngineer.Features.CriticalEffects.Patches;

[HarmonyPatch(typeof(AbstractActor), nameof(AbstractActor.FlagForDeath))]
internal static class AbstractActor_FlagForDeath_Patch
{
    [HarmonyPrefix]
    public static void Prefix(
        AbstractActor __instance,
        DeathMethod deathMethod,
        DamageType damageType,
        int stackItemID,
        string attackerID)
    {
        try
        {
            var actor = __instance;
            if (actor._flaggedForDeath)
            {
                return;
            }
            var pilot = actor.GetPilot();
            if (pilot == null || pilot.IsIncapacitated)
            {
                return;
            }
            // check for any pilot kill leftovers (the pilot was flagged but not yet incapacitated)
            if (deathMethod is not (DeathMethod.CockpitDestroyed or DeathMethod.HeadDestruction or DeathMethod.PilotKilled))
            {
                return;
            }
            // essentially copied from Mech.ApplyHeadStructureEffects
            pilot.LethalInjurePilot(actor.Combat.Constants, attackerID, stackItemID, true, damageType, null, actor.Combat.FindActorByGUID(attackerID));
            actor.GetPilot()?.ShowInjuryMessage(Strings.T("PILOT: LETHAL DAMAGE!"));
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}