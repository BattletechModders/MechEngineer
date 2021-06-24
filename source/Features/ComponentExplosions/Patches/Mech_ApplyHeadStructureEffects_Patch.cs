using BattleTech;
using Harmony;

namespace MechEngineer.Features.ComponentExplosions.Patches
{
    [HarmonyPatch(typeof(Mech), "ApplyHeadStructureEffects")]
    internal static class Mech_ApplyHeadStructureEffects_Patch
    {
        [HarmonyPriority(Priority.LowerThanNormal)]
        public static bool Prefix(Mech __instance, LocationDamageLevel oldDamageLevel, LocationDamageLevel newDamageLevel, WeaponHitInfo hitInfo)
        {
            var mech = __instance;
            if (newDamageLevel == oldDamageLevel)
            {
                return true;
            }

            if (newDamageLevel != LocationDamageLevel.Destroyed)
            {
                return true;
            }

            var attackSequence = mech.Combat.AttackDirector.GetAttackSequence(hitInfo.attackSequenceId);
            if (attackSequence != null)
            {
                return true;
            }

            // do vanilla code but without using the attack sequence (as its null)
            mech.pilot.SetNeedsInjury(InjuryReason.HeadHit);
            mech.pilot.LethalInjurePilot(mech.Combat.Constants, hitInfo.attackerId, hitInfo.stackItemUID, true, DamageType.HeadShot, null, null);
            mech.Combat.MessageCenter.PublishMessage(new AddSequenceToStackMessage(new ShowActorInfoSequence(mech, "PILOT: LETHAL DAMAGE!", FloatieMessage.MessageNature.PilotInjury, true)));

            return false;
        }
    }
}