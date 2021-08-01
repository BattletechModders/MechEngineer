using BattleTech;

namespace MechEngineer.Misc
{
    internal static class InjuryUtils
    {
        internal static void InjurePilot(AbstractActor actor, string sourceID, int stackItemUID, InjuryReason reason, DamageType damageType)
        {
            var pilot = actor.GetPilot();
            if (pilot == null)
            {
                return;
            }

            if (pilot.NeedsInjury)
            {
                Control.Logger.Warning.Log($"Can't apply heat injury as another injury is already queued, conflicting res={pilot.injuryReason} desc={pilot.InjuryReasonDescription}");
                return;
            }

            var sourceActor = actor.Combat.FindActorByGUID(sourceID);

            // TBAS: keep track of health for resist detection
            var health = pilot.Health;
            // TBAS: SetNeedsInjury (and therefore ClearNeedsInjury) is needed for resistance detection
            pilot.SetNeedsInjury(reason);
            // injure the pilot
            pilot.InjurePilot(sourceID, stackItemUID, 1, damageType, default, sourceActor);
            // TBAS: clear SetsNeedsInjury
            pilot.ClearNeedsInjury();
            // TBAS: made the pilot resist the injury
            if (health == pilot.Health)
            {
                return;
            }

            PlayAudio(actor);
        }

        private static void PlayAudio(AbstractActor actor)
        {
            // from MechShutdownSequence.CheckForHeatDamage
            if (actor.GetPilot().IsIncapacitated)
            {
                return;
            }

            AudioEventManager.SetPilotVOSwitch(AudioSwitch_dialog_dark_light.dark, actor);
            AudioEventManager.PlayPilotVO(VOEvents.Pilot_TakeDamage, actor);
            if (actor.team.LocalPlayerControlsTeam)
            {
                AudioEventManager.PlayAudioEvent("audioeventdef_musictriggers_combat", "friendly_warrior_injured");
            }
        }
    }
}
