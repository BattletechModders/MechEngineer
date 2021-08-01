using BattleTech;
using MechEngineer.Features.ShutdownInjuryProtection.Patches;

namespace MechEngineer.Features.ShutdownInjuryProtection
{
    internal class ShutdownInjuryProtectionFeature : Feature<ShutdownInjuryProtectionSettings>
    {
        internal static ShutdownInjuryProtectionFeature Shared = new();

        internal override bool Enabled => base.Enabled && (settings.ShutdownInjuryEnabled || settings.HeatDamageInjuryEnabled);

        internal override ShutdownInjuryProtectionSettings Settings => Control.settings.ShutdownInjuryProtection;

        internal static ShutdownInjuryProtectionSettings settings => Shared.Settings;

        internal static void InjurePilot(Mech mech, string sourceID, int stackItemUID)
        {
            if (!mech.IsOverheated)
            {
                return;
            }

            var pilot = mech.pilot;
            if (pilot == null)
            {
                return;
            }

            if (pilot.NeedsInjury)
            {
                Control.Logger.Warning.Log($"Can't apply heat injury as another injury is already queued, conflicting res={pilot.injuryReason} desc={pilot.InjuryReasonDescription}");
                return;
            }

            AbstractActor sourceActor;
            DamageType damageType;
            if (mech.GUID == sourceID)
            {
                sourceActor = mech;
                damageType = DamageType.OverheatSelf;
            }
            else
            {
                sourceActor = mech.Combat.FindActorByGUID(sourceID);
                damageType = DamageType.Overheat;
            }

            // TBAS: keep track of health for resist detection
            var health = pilot.Health;
            // SetNeedsInjury (and therefore ClearNeedsInjury) is needed for the TBAS mod
            pilot.SetNeedsInjury(Pilot_InjuryReasonDescription_Patch.InjuryReasonOverheated);
            // injure the pilot
            pilot.InjurePilot(sourceID, stackItemUID, 1, damageType, default, sourceActor);
            // TBAS clear SetsNeedsInjury
            pilot.ClearNeedsInjury();
            // TBAS made the pilot resist the injury
            if (health == pilot.Health)
            {
                return;
            }

            if (settings.InjuryAudioEnabled)
            {
                PlayAudio(mech);
            }
        }

        private static void PlayAudio(Mech mech)
        {
            // from MechShutdownSequence.CheckForHeatDamage
            if (mech.pilot.IsIncapacitated)
            {
                return;
            }

            AudioEventManager.SetPilotVOSwitch(AudioSwitch_dialog_dark_light.dark, mech);
            AudioEventManager.PlayPilotVO(VOEvents.Pilot_TakeDamage, mech);
            if (mech.team.LocalPlayerControlsTeam)
            {
                AudioEventManager.PlayAudioEvent("audioeventdef_musictriggers_combat", "friendly_warrior_injured");
            }
        }
    }
}