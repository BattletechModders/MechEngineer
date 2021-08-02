using System.Linq;
using BattleTech;

namespace MechEngineer.Misc
{
    internal static class InjuryUtils
    {
        internal static void SetInjury(AbstractActor actor, string sourceID, int stackItemUID, InjuryReason reason)
        {
            var pilot = actor.GetPilot();
            if (pilot == null)
            {
                return;
            }

            // check if we might override an existing injury
            if (pilot.NeedsInjury)
            {
                // TODO use a temp var and switcharoo instead
                Control.Logger.Warning.Log($"Can't apply heat injury as another injury is already queued, conflicting res={pilot.injuryReason} desc={pilot.InjuryReasonDescription}");
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
                CheckPilotStatusForInjuries(actor, sourceID, stackItemUID);
            }
        }

        internal static void CheckPilotStatusForInjuries(AbstractActor actor, string sourceID, int stackItemUID)
        {
            // if actor.GUID == sourceID -> self inflicting, probably always the case when outside an attack sequence
            actor.CheckPilotStatusFromAttack(sourceID, -1, stackItemUID);
        }

        // TODO workarounds are still mainly here because of mods
        // - vanilla does not destroy single components outside attacks
        // - in ME, overheating damage can lead to injuries, after movement and still not cooled down -> injury outside attack sequence
        // - kmissions custom activatable can destroy components outside of attack sequence
        // - other mods also can destroy components / add heat, e.g. during movements, skill checks
        public static bool AllowDelayedInjuryUntilNextTurn = true;
        public static bool ForceInjuryImmediatelyIfOutsideAttackSequence = true;
    }
}
