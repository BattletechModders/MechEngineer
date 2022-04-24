using System.Linq;
using BattleTech;
using Strings = Localize.Strings;

namespace MechEngineer.Misc;

internal static class InjuryUtils
{
    internal static void SetInjury(this AbstractActor actor, string sourceID, int stackItemUID, InjuryReason reason, DamageType damageType)
    {
        var pilot = actor.GetPilot();
        if (pilot == null)
        {
            return;
        }

        // check if we might override an existing injury
        if (pilot.NeedsInjury)
        {
            Control.Logger.Warning?.Log($"Can't apply injury as another injury is already queued, conflicting res={pilot.injuryReason} desc={pilot.InjuryReasonDescription}");
            return;
        }

        // attack sequences still going on will call CheckPilotStatusFromAttack at the end
        var hasAnyAttackSequence = actor.Combat.AttackDirector.GetAllAttackSequencesThatAffectCombatant(actor).Any();
        if (ForceInjuryImmediatelyIfOutsideAttackSequence || AllowDelayedInjuryUntilNextTurn || hasAnyAttackSequence)
        {
            pilot.SetNeedsInjury(reason);
        }
        if (ForceInjuryImmediatelyIfOutsideAttackSequence && !hasAnyAttackSequence)
        {
            CheckPilotStatusForInjuries(actor, sourceID, stackItemUID, damageType);
        }
    }

    // TODO workarounds are still mainly here because of mods
    // - vanilla does not destroy single components outside attacks
    // - in ME, overheating damage can lead to injuries, after movement and still not cooled down -> injury outside attack sequence
    // - kmissions custom activatable can destroy components outside of attack sequence
    // - other mods also can destroy components / add heat, e.g. during movements, skill checks
    private static bool AllowDelayedInjuryUntilNextTurn = true;
    private static bool ForceInjuryImmediatelyIfOutsideAttackSequence = true;

    // this is essentially a code clone from CheckPilotStatusFromAttack, but without the hardcoded damageType lookup
    private static void CheckPilotStatusForInjuries(AbstractActor actor, string sourceID, int stackItemID, DamageType damageType)
    {
        // if actor.GUID == sourceID -> self inflicting, probably always the case when outside an attack sequence
        // TBAS: requires damage types to be set
        //actor.CheckPilotStatusFromAttack(sourceID, -1, stackItemUID);

        var pilot = actor.GetPilot();
        if (pilot.IsIncapacitated || !pilot.NeedsInjury)
        {
            return;
        }

        var health = pilot.Health;
        pilot.InjurePilot(sourceID, stackItemID, 1, damageType, null, actor.Combat.FindActorByGUID(sourceID));
        var injuryIgnored = pilot.Health == health;

        if (pilot.IsIncapacitated)
        {
            pilot.ShowInjuryMessage(Strings.T("PILOT INCAPACITATED!"));
            pilot.PlayDeathMusic();
            pilot.PlayDeathVO();
            if (actor.IsDead || actor.IsFlaggedForDeath)
            {
                return;
            }
            actor.FlagForDeath("Pilot Killed", DeathMethod.PilotKilled, damageType, 1, stackItemID, sourceID, false);
            actor.HandleDeath(sourceID);
        }
        else
        {
            if (injuryIgnored)
            {
                pilot.ShowInjuryMessage(Strings.T("{0}: INJURY IGNORED", pilot.InjuryReasonDescription));
            }
            else
            {
                pilot.ShowInjuryMessage(Strings.T("{0}: PILOT INJURED", pilot.InjuryReasonDescription));
                pilot.PlayInjuryMusic();
                pilot.PlayInjuryVO();
            }
        }

        pilot.ClearNeedsInjury();
    }

    internal static void ShowInjuryMessage(this Pilot pilot, string message)
    {
        var sequence = new ShowActorInfoSequence(pilot.ParentActor, message, FloatieMessage.MessageNature.PilotInjury, true);
        pilot.ParentActor.Combat.MessageCenter.PublishMessage(new AddSequenceToStackMessage(sequence));
    }

    private static void PlayInjuryMusic(this Pilot pilot)
    {
        AudioEventManager.PlayAudioEvent(
            AudioConstantsDef.MUSICTRIGGERS_COMBAT,
            pilot.ParentActor.team.LocalPlayerControlsTeam
                ? nameof(AudioTriggerList.friendly_warrior_injured)
                : nameof(AudioTriggerList.enemy_warrior_injured)
        );
    }

    private static void PlayInjuryVO(this Pilot pilot)
    {
        AudioEventManager.SetPilotVOSwitch(AudioSwitch_dialog_dark_light.dark, pilot.ParentActor);
        AudioEventManager.PlayPilotVO(VOEvents.Pilot_TakeDamage, pilot.ParentActor);
    }
}