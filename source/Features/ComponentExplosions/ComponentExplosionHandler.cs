using System.Linq;
using BattleTech;
using CustomComponents;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    internal class ComponentExplosionHandler
    {
        internal static ComponentExplosionHandler Shared = new ComponentExplosionHandler();

        internal void CheckForExplosion(MechComponent component, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            if (!applyEffects)
            {
                return;
            }
            
            if (damageLevel != ComponentDamageLevel.Destroyed)
            {
                return;
            }

            if (!(component.componentDef.Is<ComponentExplosion>(out var exp)))
            {
                return;
            }

            var actor = component.parent;
            var mech = actor as Mech;

            var ammoCount = 0;
            if (component is AmmunitionBox box)
            {
                var adapter = new MechComponentAdapter(box);
                ammoCount = adapter.statCollection.GetValue<int>("CurrentAmmo");
            }
            else if (component is Weapon w2)
            {
                ammoCount = w2.InternalAmmo;
            }

            
            var attackSequence = actor.Combat.AttackDirector.GetAttackSequence(hitInfo.attackSequenceId);

            var heatDamage = exp.HeatDamage + ammoCount * exp.HeatDamagePerAmmo;
            //Control.mod.Logger.LogDebug($"heatDamage={heatDamage}");
            if (!Mathf.Approximately(heatDamage, 0))
            {
                actor.AddExternalHeat("AMMO EXPLOSION HEAT", (int)heatDamage);
                attackSequence?.FlagAttackDidHeatDamage();
            }

            if (mech != null)
            {
                var stabilityDamage = exp.StabilityDamage + ammoCount * exp.StabilityDamagePerAmmo;
                //Control.mod.Logger.LogDebug($"stabilityDamage={stabilityDamage}");
                if (!Mathf.Approximately(stabilityDamage, 0))
                {
                    mech.AddAbsoluteInstability(stabilityDamage, StabilityChangeSource.Effect, hitInfo.targetId);
                }
            }

            var explosionDamage = exp.ExplosionDamage + ammoCount * exp.ExplosionDamagePerAmmo;
            //Control.mod.Logger.LogDebug($"explosionDamage={explosionDamage}");

            if (Mathf.Approximately(explosionDamage, 0))
            {
                return;
            }

            if (mech == null)
            {
                // for vehicles and turrets we play dead, idea from AIM
                actor.FlagForDeath(
                    "Ammo Explosion",
                    DeathMethod.AmmoExplosion,
                    DamageType.Weapon,
                    1,
                    hitInfo.stackItemUID,
                    hitInfo.attackerId,
                    false
                );
                return;
            }

            Mech_DamageLocation_Patch.IsInternalExplosion = true;
            try
            {
                attackSequence?.FlagAttackCausedAmmoExplosion();

                actor.PublishFloatieMessage($"{component.Name} EXPLOSION");
                if (actor.Combat.Constants.PilotingConstants.InjuryFromAmmoExplosion)
                {
                    var pilot = actor.GetPilot();
                    pilot?.SetNeedsInjury(InjuryReason.AmmoExplosion);
                }

                // this is very hacky as this is an invalid weapon
                var weapon = new Weapon(mech, actor.Combat, component.mechComponentRef, component.uid);

                // bool DamageLocation(int originalHitLoc, WeaponHitInfo hitInfo, ArmorLocation aLoc, Weapon weapon, float totalDamage, int hitIndex, AttackImpactQuality impactQuality)
                var args = new object[] {component.Location, hitInfo, (ArmorLocation) component.Location, weapon, explosionDamage, 0, AttackImpactQuality.Solid, DamageType.AmmoExplosion};
                Traverse.Create(mech).Method("DamageLocation", args).GetValue();
            }
            finally
            {
                Mech_DamageLocation_Patch.IsInternalExplosion = false;
            }
        }

        internal CASEComponent GetCASEProperties(Mech mech, int location)
        {
            return mech.allComponents
                .Where(c => c.DamageLevel == ComponentDamageLevel.Functional)
                .Select(componentRef => new { componentRef, CASE = componentRef.componentDef.GetComponent<CASEComponent>() } )
                .Where(t => t.CASE != null)
                .Where(t => t.CASE.AllLocations || t.componentRef.Location == location)
                .Select(t => t.CASE)
                .OrderBy(CASE => CASE.AllLocations) // localized CASE always overrides global CASE
                .FirstOrDefault();
        }
    }

    public class MechComponentAdapter : Adapter<MechComponent>
    {
        public MechComponentAdapter(MechComponent instance) : base(instance)
        {
        }

        public StatCollection statCollection => traverse.Field("statCollection").GetValue<StatCollection>();
    }
}
